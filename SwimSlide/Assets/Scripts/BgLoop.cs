using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgLoop : MonoBehaviour
{
    public float speed;
    public Renderer bgRenderer;
    private void Update() {
        bgRenderer.material.mainTextureOffset += new Vector2(speed*Time.deltaTime,0f);
    }
}
