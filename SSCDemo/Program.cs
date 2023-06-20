using SSC;

using System;

namespace SSCDemo
{
    public interface IA
    {
        IB B { get; }
    }

    public interface IB
    {
        IC C { get; }
        ID D { get; }
    }

    public interface IC { }

    public interface ID { }

    public abstract class BaseEntity
    {
        private readonly Guid _id = Guid.Empty;

        protected BaseEntity()
        {
            if (_id == Guid.Empty)
                _id = Guid.NewGuid();
        }

        public override string ToString() => _id.ToString();
    }

    public class A : BaseEntity, IA
    {
        public IB B { get; }

        public A(IB b)
        {
            B = b;
        }
    }

    public class B : BaseEntity, IB
    {
        public IC C { get; }
        public ID D { get; }

        public B(IC c, ID d)
        {
            C = c;
            D = d;
        }
    }

    public class C : BaseEntity, IC { }

    public class D : BaseEntity, ID { }

    public class E : BaseEntity { }

    public class F : BaseEntity { }

    internal class Program
    {
        static void Main(string[] args)
        {
            var container = new SimpleServiceContainer();

            container.RegisterTransient<ID, D>();
            container.RegisterTransient<IC, C>();
            container.RegisterTransient<IB, B>();
            container.RegisterTransient<IA, A>();

            container.RegisterSingleton<E>();

            container.RegisterScoped<F>();

            var a1 = container.CreateInstance<IA>();
            var a2 = container.CreateInstance<IA>();

            var e1 = container.CreateInstance<E>();
            var e2 = container.CreateInstance<E>();

            Console.WriteLine($"[Singleton] e1 {e1}");
            Console.WriteLine($"[Singleton] e2 {e2}");
            Console.WriteLine();
            Console.WriteLine($"[Transient] a1 {a1}");
            Console.WriteLine($"[Transient] a2 {a2}");

            Console.WriteLine();
            using (var scope1 = container.CreateScope())
            {
                Console.WriteLine("-----=====Scope 1 begins=====-----");
                var f1 = scope1.GetService(typeof(F));
                var f2 = scope1.GetService(typeof(F));

                Console.WriteLine($"[Scoped] F1 {f1}");
                Console.WriteLine($"[Scoped] F2 {f2}");
                Console.WriteLine("-----=====Scope 1  ends=====-----");
            }

            Console.WriteLine();

            using (var scope1 = container.CreateScope())
            {
                Console.WriteLine("-----=====Scope 2 begins=====-----");
                var f1 = scope1.GetService(typeof(F));
                var f2 = scope1.GetService(typeof(F));

                Console.WriteLine($"[Scoped] F1 {f1}");
                Console.WriteLine($"[Scoped] F2 {f2}");
                Console.WriteLine("-----=====Scope 2  ends=====-----");
            }
        }
    }
}
