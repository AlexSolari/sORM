# sORM
Simple ORM and nothing more.

To link class and table in database you need to inherit your class from DataEntity and decorate it with DataModel attribute.
Then you need to decorate fields you want to see in database with MapAsType attributes.

Example: 
```c#
  [DataModel]
  class MyClass : DataEntity
  {
    [MapAsType(DataType.Int)]
    public int MyProperty { get; set; }
  
    [MapAsType(DataType.String)]
    public string MyProperty2 { get; set; }
  
    public override string DataId { get; set; }
  }
```

Then, on startup you need to setup connection string:

```c#
  SimpleORM.Current.Initialize(@"%CONNECTION_STRING_EXAMPLE%");
```

To create or update records use `CreateOrUpdate` method:

```c#
  var obj = new MyClass() { MyProperty = 1, MyProperty2 = "foo" };
  SimpleORM.Current.CreateOrUpdate(obj);
  obj.MyProperty = 666;
  SimpleORM.Current.CreateOrUpdate(obj);
```

To delete exact object use `Delete` method:

```c#
  SimpleORM.Current.Delete(obj);
```

You can get or delete bunch of records using **Conditions**.
**Conditions** is a way to create complex conditions using *Equals*, *NotEquals*, *More*, *Less* and *Like* operators.
**Conditions** can be modified using *And* and *Or* operators.

Example:

```c#
  SimpleORM.Current.Delete<MyClass>(
    Operator.Or(Condition.Equals("MyProperty2", "foo"), Condition.Equals("MyProperty", 2))
  );
  
  SimpleORM.Current.Delete<MyClass>(
    Operator.And(
      Condition.Equals("MyProperty2", "123"), 
      Operator.Or(
        Condition.Equals("MyProperty2", "333"), Condition.Equals("MyProperty", 999)
      )
    )
  );

  var result = SimpleORM.Current.Get<MyClass>(
    Condition.Equals("MyProperty2", "baz"),
    new DataEntityListLoadOptions() { PageSize = 1, PageNumber = 2 }
  );
```
