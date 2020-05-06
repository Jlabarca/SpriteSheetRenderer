using UnityEngine;

public class GUIHelper : MonoBehaviour{
  public void ChangeAnimation(string animationName) {
    SpriteSheetAnimator.Play(DynamicAnimationsDemo.character, animationName);
  }
}
