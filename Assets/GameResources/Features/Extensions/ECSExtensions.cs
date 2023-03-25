namespace Game.Features.Extensions
{
   using Leopotam.EcsLite;

   public static class ECSExtensions
   {
      public static ref TComponent AddComponent<TComponent>(this EcsWorld ecsWorld, int entity)
         where TComponent : struct
      {
         var pool = ecsWorld.GetPool<TComponent>();
         ref TComponent addComponent = ref pool.Add(entity);

         return ref addComponent;
      }
   }
}
