using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class SpriteSheetUvJobSystem : JobComponentSystem {
  [BurstCompile]
  struct UpdateJob : IJobForEach<SpriteIndex, BufferHook> {
    [NativeDisableParallelForRestriction]
    public DynamicBuffer<SpriteIndexBuffer> indexBuffer;
    [ReadOnly]
    public int bufferEntityId;
    public void Execute([ReadOnly, ChangedFilter] ref SpriteIndex data, [ReadOnly] ref BufferHook hook) {
      if(bufferEntityId == hook.bufferEnityID)
        indexBuffer[hook.bufferID] = data.Value;
    }
  }

  protected override JobHandle OnUpdate(JobHandle inputDeps) {
    var buffers = DynamicBufferManager.GetIndexBuffers();
    var jobs = new NativeArray<JobHandle>(buffers.Length, Allocator.Persistent);
   // Debug.Log(buffers[0].Length +"-"+buffers[1].Length +"-"+buffers[2].Length +"-"+buffers[3].Length);
    for(var i = 0; i < buffers.Length; i++) {
      Debug.Log(i);
      inputDeps = new UpdateJob() {
        indexBuffer = buffers[i],
        bufferEntityId = i
      }.Schedule(this, inputDeps);
      jobs[i] = inputDeps;
    }
    JobHandle.CompleteAll(jobs);
    jobs.Dispose();
    return inputDeps;
  }
}
