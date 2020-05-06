using System;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetAnimationSystem : JobComponentSystem {
  [BurstCompile]
  struct SpriteSheetAnimationJob : IJobForEach<SpriteSheetAnimation, SpriteIndex> {

    public void Execute(ref SpriteSheetAnimation animCmp, ref SpriteIndex spriteSheetCmp)
    {
      //Debug.Log("Execute "+animCmp.elapsedFrames);
      if(animCmp.play && animCmp.elapsedFrames % animCmp.samples == 0 && animCmp.elapsedFrames != 0) {
        switch(animCmp.repetition) {
          case SpriteSheetAnimation.RepetitionType.Once:
            if(!NextWillReachEnd(animCmp, spriteSheetCmp)) {
              spriteSheetCmp.Value++;
            }
            else {
              animCmp.play = false;
              animCmp.elapsedFrames = 0;
            }
            break;
          case SpriteSheetAnimation.RepetitionType.Loop:
            spriteSheetCmp.Value = 1;
            break;
          case SpriteSheetAnimation.RepetitionType.PingPong:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        animCmp.elapsedFrames = 0;
      }
      else if(animCmp.play) {
        animCmp.elapsedFrames++;
      }
    }

    private static bool NextWillReachEnd(SpriteSheetAnimation anim, SpriteIndex sprite) {
      return sprite.Value++ >= anim.maxSprites;
    }
  }

  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var job = new SpriteSheetAnimationJob();
    return job.Schedule(this, inputDeps);
  }
}

