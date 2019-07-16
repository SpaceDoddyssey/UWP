using UnityEngine;
using System.Collections;

/*******************************************************
 * Class:           AIConeDetection
 * Description:     Description begin here
 * 
 * Studio Leaves (c) 
 *******************************************************/
public class AIConeDetection : MonoBehaviour {

    public GameObject chairObj;
    public GameObject triggerObj; // MUST be a gameobject that has a Triggerable script attached - Trigger() will be called when player enters line of sight

    /* Fov Properties */
	public  bool 		 m_bIs2D  					  = false;
    public  float 		 m_fConeLenght 	              = 5.0f;
    public float         m_fHorzAngleOfView           = 90.0f;
    public float         m_fVertAngleOfView           = 30.0f;
    public float         m_vStartDistanceCone         = 2.0f;
    public  Material     m_matVisibilityCone          = null;
    public  bool 		 m_bHasStartDistance          = true;
    public  LayerMask    m_MaskLayerToIgnore;
    private float        m_fFixedCheckNextTime;

    /* Render Properties */
    public  bool         m_bShowCone                     = true;
    public  int          m_iHorzConeVisibilityPrecision  = 10;
    public  int          m_iVertConeVisibilityPrecision  = 4;
    //public  float       m_fDistanceForRender           = 600.0f;

    private Mesh         m_mConeMesh;
    private Vector3[]    m_vVertices;
    private Vector2[]    m_vUV;
    private Vector3[]    m_vNormals;
    private int[]	     m_iTriangles;
    private GameObject   m_goVisibilityCone          = null;
    private int 		 m_iVertMax                  = 120;
    private int			 m_iTrianglesMax             = 120;

    private float        m_fHorzSpan;
    private float        m_fHorzStartRadians;
    private float        m_fHorzCurrentRadians;
    //private float      m_fConeLenghtFixed;

    private float       m_fVertSpan;
    private float       m_fVertStartRadians;
    private float       m_fVertCurrentRadians;

    private ArrayList   m_goGameObjectIntoCone;
    public  ArrayList   GameObjectIntoCone {
        get { return m_goGameObjectIntoCone; }
    }

	public bool Is2DCone {
		get { return m_bIs2D; }
	}

	void Start () {
	    InitAIConeDetection();
	}
	
	void Update () {
        UpdateAIConeDetection();
	}

    private void InitAIConeDetection() {
        m_goGameObjectIntoCone  = new ArrayList();
        m_goVisibilityCone      = GameObject.CreatePrimitive( PrimitiveType.Cube );
        Component.Destroy( m_goVisibilityCone.GetComponent<BoxCollider>() );

        m_goVisibilityCone.name                             = this.name + "_VisConeMesh";
        m_mConeMesh                                         = new Mesh();
        m_goVisibilityCone.GetComponent<MeshFilter>().mesh  = m_mConeMesh;

        m_iVertMax = m_iHorzConeVisibilityPrecision * 2 + 2;
        m_iTrianglesMax = m_iHorzConeVisibilityPrecision * 2;

        // test for 3d cones
        //m_iTrianglesMax = (m_iHorzConeVisibilityPrecision + 1) * (m_iVertConeVisibilityPrecision + 1) * 2;
        //m_iVertMax = m_iTrianglesMax + 2;

        m_vVertices = new Vector3[  m_iVertMax          ];
        m_iTriangles    = new int    [  m_iTrianglesMax * 3 ];
        m_vNormals      = new Vector3[  m_iVertMax          ];

        m_mConeMesh.vertices  = m_vVertices;
        m_mConeMesh.triangles = m_iTriangles;
        m_mConeMesh.normals   = m_vNormals;

        m_vUV           = new Vector2[ m_mConeMesh.vertices.Length ];
        m_mConeMesh.uv  = m_vUV;

        m_goVisibilityCone.GetComponent<Renderer>().material = m_matVisibilityCone;

        for ( int i = 0; i < m_iVertMax; ++i ) {
            m_vNormals[ i ] = Vector3.up;
        }

        m_fHorzStartRadians     = ( 360.0f - ( m_fHorzAngleOfView ) ) * Mathf.Deg2Rad;
        m_fHorzCurrentRadians = m_fHorzStartRadians;
        m_fHorzSpan = (m_fHorzAngleOfView) / m_iHorzConeVisibilityPrecision;
        m_fHorzSpan *= Mathf.Deg2Rad;
        m_fHorzSpan *= 2.0f;
        //m_fHorzAngleOfView *= 0.5f;
        //m_fConeLenghtFixed  = m_fConeLenght * m_fConeLenght;

        m_fVertStartRadians = (360.0f - (m_fVertAngleOfView)) * Mathf.Deg2Rad;
        m_fVertCurrentRadians = m_fVertStartRadians;
        m_fVertSpan = (m_fVertAngleOfView) / m_iVertConeVisibilityPrecision;
        m_fVertSpan *= Mathf.Deg2Rad;
        m_fVertSpan *= 2.0f;
        //m_fVertAngleOfView *= 0.5f;
    }

    private void UpdateAIConeDetection() {
        DrawVisibilityCone2();
        if (GameObjectIntoCone.Contains(chairObj)) {
            Debug.Log("I see you!");
            Triggerable trigScript = triggerObj.GetComponent<Triggerable>();
            trigScript.Trigger();
        }
    }

    public void DisableCone() {
        m_mConeMesh.Clear();
    }

    private RaycastHit  m_rcInfo;
    private Ray         m_rayDir = new Ray();

    private void DrawVisibilityCone2() {
        m_goGameObjectIntoCone.Clear();

        m_fHorzCurrentRadians = m_fHorzStartRadians;
        m_fVertCurrentRadians = m_fVertStartRadians;
        Vector3 CurrentVector 		= this.transform.forward;
        //Vector3 DrawVectorCurrent 	= this.transform.forward;

        int index = 0;
        for ( int i = 0; i < m_iHorzConeVisibilityPrecision + 1; ++i ) {
            testSlice(CurrentVector, m_fHorzCurrentRadians, m_fVertCurrentRadians, index);
            m_fHorzCurrentRadians += m_fHorzSpan;
            index += 2;
        }

        if ( m_bShowCone ) {
            int localIndex = 0;
            for ( int j = 0; j < m_iTrianglesMax * 3; j = j + 6 ) {
                m_iTriangles[ j     ] = localIndex;
                m_iTriangles[ j + 1 ] = localIndex + 3;
                m_iTriangles[ j + 2 ] = localIndex + 1;

                m_iTriangles[ j + 3 ] = localIndex + 2;
                m_iTriangles[ j + 4 ] = localIndex + 3;
                m_iTriangles[ j + 5 ] = localIndex;

                localIndex += 2;
            }

            m_mConeMesh.Clear();
            m_mConeMesh.vertices  = m_vVertices;
            m_mConeMesh.triangles = m_iTriangles;
            m_mConeMesh.normals   = m_vNormals;
            m_mConeMesh.RecalculateNormals();
            ;
        }
        else {
            m_mConeMesh.Clear();
        }
        
    }

    private void testSlice(Vector3 CurrentVector, float horzAngle, float vertAngle, int index)
    {
        Vector3 DrawVectorCurrent;

        if (!m_bIs2D) {
            float newX = CurrentVector.x * Mathf.Cos(horzAngle) - CurrentVector.z * Mathf.Sin(horzAngle);
            float newZ = CurrentVector.x * Mathf.Sin(horzAngle) + CurrentVector.z * Mathf.Cos(horzAngle);
            //float newY = CurrentVector.y * Mathf.Sin( horzAngle ) + CurrentVector.z * Mathf.Cos( horzAngle );

            DrawVectorCurrent.x = newX;
            DrawVectorCurrent.y = 0.0f;
            DrawVectorCurrent.z = newZ;
        }
        else {
            float newX = CurrentVector.x * Mathf.Cos(horzAngle) - CurrentVector.y * Mathf.Sin(horzAngle);
            //float newZ = CurrentVector.y * Mathf.Sin( horzAngle ) + CurrentVector.z * Mathf.Cos( horzAngle );
            float newY = CurrentVector.x * Mathf.Sin(horzAngle) + CurrentVector.y * Mathf.Cos(horzAngle);

            DrawVectorCurrent.x = newX;
            DrawVectorCurrent.y = newY;
            DrawVectorCurrent.z = 0.0f;
        }

        //float Angle       = 90.0f;
        //DrawVectorCurrent = Quaternion.Euler( 0.0f, 0.0f, Angle ) * DrawVectorCurrent;

        /* Calcoliamo dove arriva il Ray */
        float FixedLenght = m_fConeLenght;
        bool bFoundWall = false;
        /* Adattiamo la mesh alla superfice sulla quale tocca */

        m_rayDir.origin = this.transform.position;
        m_rayDir.direction = DrawVectorCurrent.normalized;

        /* If we have the 2D support, we should check for 2D colliders */
        if (m_bIs2D) {

            Vector2 pos = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 dir = new Vector2(m_rayDir.direction.x, m_rayDir.direction.y);

            RaycastHit2D hit = Physics2D.Raycast(pos, dir, Mathf.Infinity, m_MaskLayerToIgnore);
            if (hit.collider != null) {
                if (hit.distance < m_fConeLenght) {
                    bFoundWall = true;
                    FixedLenght = hit.distance;

                    bool bGOFound = false;
                    foreach (GameObject go in m_goGameObjectIntoCone) {
                        if (go.GetInstanceID() == hit.collider.gameObject.GetInstanceID()) {
                            bGOFound = true;
                            break;
                        }
                    }
                    if (!bGOFound) {
                        m_goGameObjectIntoCone.Add(hit.collider.gameObject);
                    }
                }
            }
        }

        // if ( Physics.Raycast( m_rayDir, out m_rcInfo, Mathf.Infinity, m_MaskLayerToIgnore ) ) {
        if (Physics.Raycast(m_rayDir, out m_rcInfo, Mathf.Infinity)) {
            if (m_rcInfo.distance < m_fConeLenght) {
                bFoundWall = true;
                FixedLenght = m_rcInfo.distance;

                bool bGOFound = false;
                foreach (GameObject go in m_goGameObjectIntoCone) {
                    if (go.GetInstanceID() == m_rcInfo.collider.gameObject.GetInstanceID()) {
                        bGOFound = true;
                        break;
                    }
                }
                if (!bGOFound) {
                    m_goGameObjectIntoCone.Add(m_rcInfo.collider.gameObject);
                }
            }
        }

        if (m_bHasStartDistance) {
            m_vVertices[index] = this.transform.position + DrawVectorCurrent.normalized * m_vStartDistanceCone;
        }
        else {
            m_vVertices[index] = this.transform.position;
        }

        m_vVertices[index + 1] = this.transform.position + DrawVectorCurrent.normalized * FixedLenght;
        //m_vVertices[ index + 1 ].y  = this.transform.position.y;

        Color color;
        if (bFoundWall)
            color = Color.red;
        else
            color = Color.yellow;

        Debug.DrawLine(this.transform.position, m_vVertices[index + 1], color);
    }
}
