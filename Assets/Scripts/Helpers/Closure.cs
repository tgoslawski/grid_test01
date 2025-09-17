using System;

public struct Closure<TContext>
{
   private Delegate del;
   private TContext context;

   public Closure(Delegate del, TContext context = default)
   {
      this.del = del;
      this.context = context;
   }

   public void Invoke()
   {
      if (del is Action action)
      {
         action();
      }
      else if (del is Action<TContext> actionWithContext)
      {
         actionWithContext(context);
      }
      else
      {
         throw new InvalidOperationException("Unsupported delegate type for Invoke.");
      }
   }

   public TResult Invoke<TResult>()
   {
      if (del is Func<TResult> func)
      {
         return func();
      }
      else if (del is Func<TContext, TResult> funcWithContext)
      {
         return funcWithContext(context);
      }
      else
      {
         throw new InvalidOperationException("Unsupported delegate type for Invoke.");
      }
   }
}
