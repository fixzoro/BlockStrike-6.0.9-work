using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
[AddComponentMenu("NGUI/Interaction/Center Scroll View on Click")]
public class UICenterOnClick : MonoBehaviour
{
	// Token: 0x0600022F RID: 559 RVA: 0x000262C0 File Offset: 0x000244C0
	private void OnClick()
	{
		UICenterOnChild uicenterOnChild = NGUITools.FindInParents<UICenterOnChild>(base.gameObject);
		UIPanel uipanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		if (uicenterOnChild != null)
		{
			if (uicenterOnChild.enabled)
			{
				uicenterOnChild.CenterOn(base.transform);
			}
		}
		else if (uipanel != null && uipanel.clipping != UIDrawCall.Clipping.None)
		{
			UIScrollView component = uipanel.GetComponent<UIScrollView>();
			Vector3 pos = -uipanel.cachedTransform.InverseTransformPoint(base.transform.position);
			if (!component.canMoveHorizontally)
			{
				pos.x = uipanel.cachedTransform.localPosition.x;
			}
			if (!component.canMoveVertically)
			{
				pos.y = uipanel.cachedTransform.localPosition.y;
			}
			SpringPanel.Begin(uipanel.cachedGameObject, pos, 6f);
		}
	}
}
