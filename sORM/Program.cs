using sORM.Core;
using sORM.Core.Conditions;
using sORM.Core.Mappings;
using sORM.Core.Requests.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM
{
    class Program
    {
        [DataModel]
        class MyClass : DataEntity
        {
            [MapAsType(DataType.Int)]
            public int MyProperty { get; set; }
            [MapAsType(DataType.String)]
            public string MyProperty2 { get; set; }

            public override string DataId { get; set; }
        }

        static void Main(string[] args)
        {
            SimpleORM.Current.Initialize(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\o.halanin\Documents\test.mdf;Integrated Security=True;Connect Timeout=30");
            SimpleORM.Current.ToggleLogging();
            var obj = new MyClass() { MyProperty = 1, MyProperty2 = "foo" };
            var obj2 = new MyClass() { MyProperty = 2, MyProperty2 = "bar" };
            var obj1 = new MyClass() { MyProperty = 3, MyProperty2 = "baz" };
            var obj3 = new MyClass() { MyProperty = 4, MyProperty2 = "baz" };
            var obj4 = new MyClass() { MyProperty = 5, MyProperty2 = "baz" };

            SimpleORM.Current.CreateOrUpdate(obj);
            SimpleORM.Current.CreateOrUpdate(obj1);
            SimpleORM.Current.CreateOrUpdate(obj2);
            SimpleORM.Current.CreateOrUpdate(obj3);
            SimpleORM.Current.CreateOrUpdate(obj4);

            SimpleORM.Current.Delete<MyClass>(Operator.Or(Condition.Equals("MyProperty2", "foo"), Condition.Equals("MyProperty", 2)));
            SimpleORM.Current.Delete<MyClass>(Operator.And(Condition.Equals("MyProperty2", "123"), Operator.Or(Condition.Equals("MyProperty2", "333"), Condition.Equals("MyProperty", 999))));

            var result = SimpleORM.Current.Get<MyClass>(
                Condition.Equals("MyProperty2", "baz"),
                new DataEntityListLoadOptions() { PageSize = 1, PageNumber = 2 }
                );

            obj4.MyProperty = 666;

            SimpleORM.Current.CreateOrUpdate(obj4);

            SimpleORM.Current.Delete(obj3);
        }
    }
}
