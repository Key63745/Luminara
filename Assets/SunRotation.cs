using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{

    Transform _transform;
    Vector3 _originalPosition;
    float _timeElapsed;
    float _lerpDuration = 1000;

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        _originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _transform.position = Vector3.Lerp(_originalPosition, new Vector3(-500000f, _transform.position.y, _transform.position.z), _timeElapsed / _lerpDuration );
        _timeElapsed += Time.deltaTime;
    }
}
