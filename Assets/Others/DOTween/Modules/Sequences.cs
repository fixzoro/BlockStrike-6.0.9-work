using System;
using DG.Tweening;
using UnityEngine;

public class Sequences : MonoBehaviour
{
    public Transform target;

    private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(this.target.DOMoveY(2f, 1f, false));
		sequence.Join(this.target.DORotate(new Vector3(0f, 135f, 0f), 1f, RotateMode.Fast));
		sequence.Append(this.target.DOScaleY(0.2f, 1f));
		sequence.Insert(0f, this.target.DOMoveX(4f, sequence.Duration(true), false).SetRelative<Tweener>());
		sequence.SetLoops(4, LoopType.Yoyo);
	}
}
