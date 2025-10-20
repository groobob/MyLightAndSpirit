using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    Vector2 startPos;
    [SerializeField] float parallax = .1f;

    void Start()
    {
        startPos = transform.position;
    }


    void Update()
    {
        Vector2 point = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float Xpos = Mathf.Lerp(transform.position.x, startPos.x + (point.x * parallax), 2f * Time.deltaTime);
        float Ypos = Mathf.Lerp(transform.position.y, startPos.y + (point.y * parallax), 2f * Time.deltaTime);

        transform.position = new Vector3(Xpos, Ypos, 0);
    }
}