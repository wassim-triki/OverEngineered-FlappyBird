using UnityEngine;

public class MountainsLoop : MonoBehaviour
{
    [Header("Segments (same sprite, seamless)")]
    [SerializeField] private SpriteRenderer mountainSprite;   
    private SpriteRenderer _segmentB;                         

    [Header("Motion")]
    [SerializeField] private float speed = 0.4f;             

    [Header("Screen / Seam")]
    private Camera _cam;                                     
    [SerializeField] private float smallOverlap = 0.25f;     

    float _width;    
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

        if (_segmentB == null)
        {
            _segmentB = Instantiate(mountainSprite, mountainSprite.transform.parent);
            _segmentB.name = mountainSprite.name + "_B";
        }

        _width = mountainSprite.bounds.size.x;
        _halfW = _width * 0.5f;

        Vector3 posB = mountainSprite.transform.position;
        posB.x += _width - smallOverlap;
        _segmentB.transform.position = posB;
    }

    void Update()
    {
        if (_cam == null) return; 

        float dx = speed * Time.deltaTime;
        Move(mountainSprite.transform, -dx);
        Move(_segmentB.transform, -dx);

        float leftEdgeX = _cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;

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
