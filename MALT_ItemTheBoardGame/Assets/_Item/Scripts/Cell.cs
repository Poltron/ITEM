using UnityEngine;

public class Cell : MonoBehaviour 
{
	public int x;
	public int y;
       
	public Ball ball;

    private SpriteRenderer spriteRenderer;
	private CircleCollider2D circleCollider;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D> ();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetCollider(bool isActive)
    {
        circleCollider.enabled = isActive;
    }

    public bool HasBall()
    {
        return ball != null;
    }

    public bool HasBall(BallColor color)
    {
        if (!HasBall())
            return false;

        return ball.Color == color;
    }

    public void SetModelPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetHighlightedCell(bool isHighlighted)
    {
        animator.SetBool("Highlighted", isHighlighted);
    }

    public void PassAboveUI(bool isEnabled)
    {
        if (isEnabled)
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("AboveUI");
        else
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("Cell");
    }
}
