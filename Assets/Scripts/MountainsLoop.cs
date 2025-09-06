using UnityEngine;

public class MountainsLoop : MonoBehaviour
{
    [Header("Segments (same sprite, seamless)")]
    [SerializeField] private SpriteRenderer mountainSprite;   // A (assign in Inspector or on this GO)
    private SpriteRenderer _segmentB;                         // auto-cloned from A

    [Header("Motion")]
    [SerializeField] private float speed = 0.4f;              // world units/sec, constant

    [Header("Screen / Seam")]
    private Camera _cam;                                      // auto-grab Main if null
    [SerializeField] private float smallOverlap = 0.25f;      // world units; prevents tiny gaps

    float _width;    // world width of a segment
    float _halfW;

    void Awake()
    {
        if (_cam == null) _cam = Camera.main;

        if (mountainSprite == null)
            mountainSprite = GetComponent<SpriteRenderer>();

        if (mountainSprite == null)
        {
            Debug.LogError("MountainsLoop: Please assign a SpriteRenderer to 'mountainSprite' or put this script on the sprite object.");
            enabled = false;
            return;
        }

        // Auto-clone B if missing
        if (_segmentB == null)
        {
            _segmentB = Instantiate(mountainSprite, mountainSprite.transform.parent);
            _segmentB.name = mountainSprite.name + "_B";
        }

        // Cache world width from A (both segments share sprite/scale)
        _width = mountainSprite.bounds.size.x;
        _halfW = _width * 0.5f;

        // Place B immediately to the right of A
        Vector3 posB = mountainSprite.transform.position;
        posB.x += _width - smallOverlap;
        _segmentB.transform.position = posB;
    }

    void Update()
    {
        if (_cam == null) return; // avoid NRE if no MainCamera

        float dx = speed * Time.deltaTime;
        Move(mountainSprite.transform, -dx);
        Move(_segmentB.transform, -dx);

        // Left screen edge (world coords)
        float leftEdgeX = _cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;

        // If a segment’s right edge is left of the screen, move it to the right of the other
        RecycleIfOffscreen(mountainSprite, _segmentB, leftEdgeX);
        RecycleIfOffscreen(_segmentB, mountainSprite, leftEdgeX);
    }

    void Move(Transform t, float dx)
    {
        t.position += new Vector3(dx, 0f, 0f);
    }

    void RecycleIfOffscreen(SpriteRenderer sr, SpriteRenderer other, float leftEdgeX)
    {
        float srRight = sr.transform.position.x + _halfW;
        if (srRight < leftEdgeX)
        {
            float newX = other.transform.position.x + _width - smallOverlap;
            var p = sr.transform.position;
            p.x = newX;
            sr.transform.position = p;
        }
    }
}
