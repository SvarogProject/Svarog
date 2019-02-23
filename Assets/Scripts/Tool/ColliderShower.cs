using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderShower : MonoBehaviour {
    
    public BoxCollider2D Collider;
    
    //private GameObject _colliderShowPlace;
    private string _colliderType;
    private MeshFilter _meshFilter;
    private Material _material;

    private MeshRenderer _meshRenderer;

    // private ArrayList _allVertices = new ArrayList();
    // private ArrayList _allTriangle = new ArrayList();

    private void Start() {

        /*
        _colliderShowPlace =
            Instantiate(Resources.Load("ColliderShowPlace"), transform.position, Quaternion.identity) as GameObject;

        _colliderType = Collider.gameObject.tag;

        if (_colliderShowPlace != null) {
            _meshFilter = _colliderShowPlace.GetComponent<MeshFilter>();
            _material = _colliderShowPlace.GetComponent<MeshRenderer>().material;
            _meshRenderer = _colliderShowPlace.GetComponent<MeshRenderer>();
        }
        */
        _colliderType = Collider.gameObject.tag;
        _meshFilter = Collider.GetComponent<MeshFilter>();
        _material = Collider.GetComponent<MeshRenderer>().material;
        _meshRenderer = Collider.GetComponent<MeshRenderer>();

        _meshRenderer.sortingLayerName = "Effect"; // 面板上没有设置的地方，但是是可以设置的

        switch (_colliderType) {
            case "MovementCollider":
                _material.color = new Color32(255, 200, 0, 100);

                break;
            case "AttackCollider":
                _material.color = new Color32(255, 50, 0, 100);

                break;
            case "HurtCollider":
                _material.color = new Color32(0, 100, 255, 100);

                break;
            case "DefenseCollider":
                _material.color = new Color32(0, 100, 0, 100);
                break;
        }

    }

    public void FixedUpdate() {

        // TODO Sprite的移动修正
        //_colliderShowPlace.transform.position = Collider.transform.position;
        //_colliderShowPlace.transform.localScale = Collider.transform.localScale;
        
        //transform.position = Collider.transform.position;
        //transform.localScale = Collider.transform.localScale;

        if (Collider.gameObject.activeSelf && Collider.enabled) {         
        
            _meshFilter.mesh.Clear();
            _meshFilter.mesh = new Mesh {
                vertices = GetBoxColliderVertexPositions(Collider),
                triangles = new[] {
                    0, 1, 2,
                    1, 2, 3
                }
            };
           
        } else {
            _meshFilter.mesh.Clear();
        }

    }

    private Vector3[] GetBoxColliderVertexPositions(BoxCollider2D boxcollider) {
        var vertices = new Vector3[4];

        vertices[0] = boxcollider.offset + new Vector2(boxcollider.size.x / 2, boxcollider.size.y / 2);

        vertices[1] = boxcollider.offset + new Vector2(boxcollider.size.x / 2, -boxcollider.size.y / 2);

        vertices[2] = boxcollider.offset + new Vector2(-boxcollider.size.x / 2, boxcollider.size.y / 2);

        vertices[3] = boxcollider.offset + new Vector2(-boxcollider.size.x / 2, -boxcollider.size.y / 2);

        return vertices;
    }
}