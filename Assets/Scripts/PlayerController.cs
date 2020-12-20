using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gravity = -20f;

    [Header("Collisions")]
    [SerializeField] private LayerMask collideWith;
    [SerializeField] private int verticalRayAmount = 4;

    private BoxCollider2D _boxCollider2D;
    private PlayerConditions _conditions;

    private Vector2 _boundsBottomLeft;
    private Vector2 _boundsBottomRight;
    private Vector2 _boundsTopLeft;
    private Vector2 _boundsTopRight;

    private float _boundsHeight;
    private float _boundsWidth;

    private float _currentGravity;
    private Vector2 _force;
    private Vector2 _nextPosition;
    private float _spriteBuffer = 0.05f;

    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();

        _conditions = new PlayerConditions();
        _conditions.Reset();
    }

    void Update()
    {
        ApplyGravity();
        StartMovement();

        SetRaycastBounds();
        CollisionBelow();

        transform.Translate(_nextPosition, Space.Self);

        SetRaycastBounds();
        CalculateMovement();
    }

    private void CollisionBelow()
    {
        if (_nextPosition.y < 0)
        {
            _conditions.IsFalling = true;
        }
        else
        {
            _conditions.IsFalling = false;
        }

        if (!_conditions.IsFalling)
        {
            _conditions.IsCollidingBelow = false;
            return;
        }

        // Calculate ray length
        float rayLength = _boundsHeight / 2f + _spriteBuffer;

        if (_nextPosition.y < 0) // Player is falling
        {
            rayLength += Mathf.Abs(_nextPosition.y);
        }

        // Calculate ray origin
        Vector2 leftOrigin = (_boundsBottomLeft + _boundsTopLeft) / 2f;
        Vector2 rightOrigin = (_boundsBottomRight + _boundsTopRight) / 2f;

        leftOrigin += (Vector2)(transform.up * _spriteBuffer) + (Vector2)(transform.right * _nextPosition.x);
        rightOrigin += (Vector2)(transform.up * _spriteBuffer) + (Vector2)(transform.right * _nextPosition.x);

        // Raycast
        for (int i = 0; i < verticalRayAmount; i++)
        {
            Vector2 rayOrigin = Vector2.Lerp(leftOrigin, rightOrigin, i / (float)(verticalRayAmount - 1));
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -transform.up, rayLength, collideWith);
            Debug.DrawRay(rayOrigin, -transform.up * rayLength, Color.green);

            if (hit) // Raycast was succesful
            {
                _nextPosition.y = -hit.distance + _boundsHeight / 2f + _spriteBuffer;

                _conditions.IsCollidingBelow = true;

                if (Mathf.Abs(_nextPosition.y) < 0.0001f)
                {
                    _nextPosition.y = 0f;
                }
            }
            else
            {
                _conditions.IsCollidingBelow = false;
            }
        }
    }

    /// <summary>
    /// Clamp the force applied
    /// </summary>
    private void CalculateMovement()
    {
        if (Time.deltaTime > 0)
        {
            _force = _nextPosition / Time.deltaTime; // Make sure the applied force is always relative to our next position
        }
    }

    /// <summary>
    /// Initialize nextPosition
    /// </summary>
    private void StartMovement()
    {
        _nextPosition = _force * Time.deltaTime;
        _conditions.Reset(); // Reset player conditions at the beginning of each tick
    }

    public void SetHorizontalForce(float xForce)
    {
        _force.x = xForce;
    }

    /// <summary>
    /// Set and caluclate the gravity to apply
    /// </summary>
    private void ApplyGravity()
    {
        _currentGravity = gravity;
        _force.y += _currentGravity * Time.deltaTime;
    }

    /// <summary>
    /// Calculate ray origins based on the box collider
    /// </summary>
    private void SetRaycastBounds()
    {
        Bounds playerBounds = _boxCollider2D.bounds;

        _boundsBottomLeft = new Vector2(playerBounds.min.x, playerBounds.min.y);
        _boundsBottomRight = new Vector2(playerBounds.max.x, playerBounds.min.y);
        _boundsTopLeft = new Vector2(playerBounds.min.x, playerBounds.max.y);
        _boundsTopRight = new Vector2(playerBounds.max.x, playerBounds.max.y);

        _boundsHeight = Vector2.Distance(_boundsBottomLeft, _boundsTopLeft);
        _boundsWidth = Vector2.Distance(_boundsBottomLeft, _boundsBottomRight);
    }
}
