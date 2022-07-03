using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nunchuck : MonoBehaviour
{
    public float Speed = 2;
    Vector3 _startPosition, _targetPosition;
    bool _movingToTarget = true;
    public float RotationalVelocity;

    LineRenderer _lineRenderer;

    void Start() {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        InvokeRepeating(nameof(LineFlicker),0,.15f);    

        _startPosition = transform.position;
        if(transform.childCount > 3)
            _targetPosition = transform.GetChild(3).position;
        else 
            _targetPosition = _startPosition;
    }

    void Update() {
        transform.Rotate(new Vector3(0,0,RotationalVelocity*Time.deltaTime));
        transform.position = Vector2.MoveTowards(transform.position, _movingToTarget ? _targetPosition : _startPosition, Speed*Time.deltaTime);
        if((transform.position == _targetPosition && _movingToTarget) || (transform.position == _startPosition && !_movingToTarget))
            _movingToTarget = !_movingToTarget;
    }

    void LineFlicker() {
        _lineRenderer.SetPositions(new Vector3[]{
            new Vector3(0,-0.5f*transform.localScale.y,0),
            new Vector3(Random.Range(-.1f,.1f), -0.25f*transform.localScale.y, 0),
            new Vector3(Random.Range(-.1f,.1f), 0,                             0),
            new Vector3(Random.Range(-.1f,.1f), 0.25f*transform.localScale.y,  0),
            new Vector3(0,0.5f*transform.localScale.y,0)
        });
    }
}