using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grapple : MonoBehaviour
{
    const string PLATFORM = "Platform";
    const string PLAYER = "Player";
    const string RELEASESWING = "Jump";

    private Vector3 _mousePos;
    private Camera _camera;
    
    private bool _isGrappling = false;
    private bool _hasShotGrapple = false;

    private DistanceJoint2D _distanceJoint;
    private LineRenderer _lineRenderer;


    private Vector3 _anchorPoint;
    private Vector2 _rayCastOffset;
    private Vector2 _grappleDestination;


    private Rigidbody2D _rbReference;
    private MovementBehavior _movementBehaviorRef;

    private float _grappleTimer = 5.0f;
    private float _currentGrappleTimer = 0.0f;

    private float _lineWidth;
    private float _lineFinalLength = 10f;
    private float _lineCurrentLength = 0f;
    private float _lineGrowSpeed = 50f;

    [SerializeField] private LevelEnd[] _levelEnds;

    private void Awake()
    {
        _rbReference = GetComponent<Rigidbody2D>();
        _movementBehaviorRef = GetComponent<MovementBehavior>();
    }

    void Start()
    {
        
        _camera = Camera.main;

        _distanceJoint = GetComponent<DistanceJoint2D>();
        _distanceJoint.enabled = false;

        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;

        _rayCastOffset.x = _rbReference.GetComponent<CapsuleCollider2D>().size.x;
        _rayCastOffset.y = _rbReference.GetComponent<CapsuleCollider2D>().size.y;

        _lineWidth = _lineRenderer.endWidth;
       
    }
   

    // Update is called once per frame
    void Update()
    {
        //Check wheter the game has ended
        foreach (LevelEnd item in _levelEnds)
        {
            if (item != null)
            {
                if (item.IsEnded)
                {
                    return;
                }
            }
        }

        GetMousePos();
        UpdateGrappleTimer();
      

        if (Input.GetMouseButtonDown(0) && !_isGrappling)
        {
            _hasShotGrapple = true;

            //Where to raycast
            _grappleDestination = new Vector2(_mousePos.x, _mousePos.y);

            //Amount of vertices
            _lineRenderer.positionCount = 4;
        }
        else if(_isGrappling && Input.GetButtonDown(RELEASESWING))
        {
            Detach();
        }

        if (_hasShotGrapple)
        {
            ShootGrapple();
            
        }

        DrawRope();

    }

    private void GetMousePos()
    {
        _mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = 0;
 
    }

    private void DrawRope()
    {
        if (_lineRenderer.positionCount <= 0)
            return;

        Vector3 playerToAnchor = _anchorPoint - transform.position;

        //Player pos anchor (start rope)
        _lineRenderer.SetPosition(0, transform.position);

        //Middle anchors (middle rope)
        _lineRenderer.SetPosition(1, transform.position + playerToAnchor / 2);

        _lineRenderer.SetPosition(2, transform.position + playerToAnchor / 2);

        //end anchor (end rope)
        _lineRenderer.SetPosition(3, _anchorPoint);

        //Interpolate the rope color from white to red
        _lineRenderer.startColor = Color.Lerp(Color.white, Color.red, _currentGrappleTimer / _grappleTimer);
        _lineRenderer.endColor = Color.Lerp(Color.white, Color.red, _currentGrappleTimer / _grappleTimer);
  
        //In case it's time to cut the rope 
        if (_currentGrappleTimer >= _grappleTimer)
        {
            ShrinkRope();
        }
    }
    
    private void UpdateGrappleTimer()
    {
        // The grapple rope gets destroyed after _grappleTimer seconds

        if (!_isGrappling)
        {
            _currentGrappleTimer = 0f;
            return;
        }

        if (_currentGrappleTimer >=_grappleTimer)
        {
            _currentGrappleTimer = _grappleTimer;
        }
        else
        {
            _currentGrappleTimer+= Time.deltaTime;
        }
    }

    void ShrinkRope()
    {
        AnimationCurve curve = new AnimationCurve();
        float width = _lineRenderer.startWidth;

        //Makes the rope look like it's being cut
        _lineWidth-=Time.deltaTime * 0.1f;
      
        if (_lineWidth < 0f)
        {
            _lineWidth = width;
            Detach();
  
        }

        curve.AddKey(0f,width);
        curve.AddKey(0.2f,_lineWidth);
        curve.AddKey(0.8f, _lineWidth);
        curve.AddKey(1f, width);

        _lineRenderer.widthCurve = curve;
    }

    void Detach()
    {
        _distanceJoint.enabled = false;
        _isGrappling = false;
        _lineRenderer.positionCount = 0;
        _movementBehaviorRef.IsGrappling = false;

        //Reset the rope visuals
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, _lineRenderer.startWidth);
        curve.AddKey(0.2f, _lineRenderer.startWidth);
        curve.AddKey(0.8f, _lineRenderer.startWidth);
        curve.AddKey(1f, _lineRenderer.startWidth);

        _lineRenderer.widthCurve = curve;
        _hasShotGrapple = false;
        _lineRenderer.positionCount = 0;
        _lineCurrentLength = 0;
    }

    void ShootGrapple()
    {
        if (_distanceJoint == null)
            return;
        if (_movementBehaviorRef == null)
            return;

        //If the rope doesn't hit anything _lineFinalLength distance further, then we missed
        if (_lineCurrentLength >= _lineFinalLength)
        {
            _hasShotGrapple = false;
             _lineRenderer.positionCount = 0;
            _lineCurrentLength = 0;
            return;
        }

        //Calculate point where to raycast
        Vector2 playerToMouse = _grappleDestination - _rbReference.position;
        Vector2 finalPoint = _rbReference.position + (playerToMouse.normalized * _lineCurrentLength - _rbReference.position);

        //Grow the line every frame
        _lineCurrentLength += Time.deltaTime * _lineGrowSpeed;

        //First raycast, hits itself (player)
        RaycastHit2D hitInfo = Physics2D.Raycast(_rbReference.position, finalPoint);
      
        //Raycast hits player , so + some offset
        Vector2 offset = Vector2.zero;
        if (hitInfo.collider.name == PLAYER)
        {
           offset = _rbReference.position + finalPoint.normalized * _rayCastOffset;
             
        }

        //Next raycast, starts from outside the player collider
        hitInfo = Physics2D.Raycast(offset, finalPoint,_lineCurrentLength);
      
        //Current anchor point, used for drawing the rope
        _anchorPoint = offset + finalPoint;

        if (hitInfo.collider != null)
        {
            //If we hit a platform, we hook onto it
            if (hitInfo.collider.gameObject.tag == PLATFORM)
            {
                _distanceJoint.enabled = true;
                _distanceJoint.connectedAnchor = hitInfo.point;
                _isGrappling = true;
                _lineRenderer.positionCount = 4;
                _anchorPoint = hitInfo.point;
                _movementBehaviorRef.IsGrappling = true;
                _movementBehaviorRef.AnchorPoint = _distanceJoint.connectedAnchor;
                _hasShotGrapple = false;

            }

        }

    }


}
