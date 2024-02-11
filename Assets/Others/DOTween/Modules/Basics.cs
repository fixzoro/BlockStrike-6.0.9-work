using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class Basics : MonoBehaviour
{
    public Transform cubeA;

    public Transform cubeB;

    private void Start()
	{
		DOTween.Init(new bool?(false), new bool?(true), new LogBehaviour?(LogBehaviour.ErrorsOnly));
		this.cubeA.DOMove(new Vector3(-2f, 2f, 0f), 1f, false).SetRelative<Tweener>().SetLoops(-1, LoopType.Yoyo);
		DOTween.To(() => this.cubeB.position, delegate(Vector3 x)
		{
			this.cubeB.position = x;
		}, new Vector3(-2f, 2f, 0f), 1f).SetRelative<TweenerCore<Vector3, Vector3, VectorOptions>>().SetLoops(-1, LoopType.Yoyo);
	}

}
