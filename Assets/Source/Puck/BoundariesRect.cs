using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>A RectTransform that creates a Physics2D Edge Collider along each edge</summary>
public class BoundariesRect : MonoBehaviour {
    private EdgeCollider2D _edges;
    [SerializeField] private List<Transform> edgePoints;
    private Vector2[] _points = new Vector2[5];

    private void Awake()
    {
        this._edges = this.gameObject.AddComponent<EdgeCollider2D>();

        for (int i = 0; i < edgePoints.Count; i++) {
            _points[i] = (Vector2)this.edgePoints[i].position;
        }

        this._edges.points = this._points;
    }

    private void Update()
    {
        
        for (int i = 0; i < edgePoints.Count; i++) {
            _points[i] = (Vector2)this.edgePoints[i].position;
            _points[i] = new Vector2(Mathf.Round(_points[i].x), Mathf.Round(_points[i].y));
        }

        this._edges.points = this._points;
    }
}