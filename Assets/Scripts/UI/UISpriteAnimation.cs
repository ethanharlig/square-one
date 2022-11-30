using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Image image;

    public Sprite[] sprites;
    public float secondsSpriteShown = 0.3f;

    private int spriteNdx;

    void Start()
    {
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(secondsSpriteShown);
        if (spriteNdx >= sprites.Length)
        {
            spriteNdx = 0;
        }
        image.sprite = sprites[spriteNdx];
        spriteNdx += 1;

        StartCoroutine(PlayAnimation());
    }
}