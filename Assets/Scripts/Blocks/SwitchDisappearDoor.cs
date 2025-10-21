using UnityEngine;

public class SwitchDisappearDoor : InteractableBlock
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    public override void ShineDeinteract()
    {
        //
    }

    public override void ShineInteract()
    {
        //
    }
    void Start()
    {
        base.init();
        //fullyDisable();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void appear()
    {
        spriteRenderer.sprite = onSprite;
        changeVisibility(true);
    }

    public void disappear()
    {
        spriteRenderer.sprite = offSprite;
        changeVisibility(false);
    }
}
