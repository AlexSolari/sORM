# sORM
Simple ORM and nothing more.

### Mappings
To link class and table in database you need to inherit your class from `DataEntity` and decorate it with `DataModel` attribute.
Then you need to decorate fields you want to see in database with `MapAsType` attributes.
You can also use `MapAuto` attribute to let the ORM detect and map type.

Supported types for auto mapping:

* `string` as **VARCHAR(MAX)**
* `int` as **INT**
* `bool` as **BIT**
* `float` as **REAL**

Example: 
```c#
  [DataModel]
  class MyClass : DataEntity
  {
    [MapAuto]
    public int MyProperty { get; set; }
  
    [MapAsType(DataType.String)]
    public string MyProperty2 { get; set; }
  
    public override string DataId { get; set; }
  }
```

### Connection setup
On startup you need to setup connection string:

```c#
  SimpleORM.Current.Initialize(@"%CONNECTION_STRING_EXAMPLE%");
```

### Creating and updating objects
To create or update records use `CreateOrUpdate` method:

```c#
  var obj = new MyClass() { MyProperty = 1, MyProperty2 = "foo" };
  SimpleORM.Current.CreateOrUpdate(obj);
  obj.MyProperty = 666;
  SimpleORM.Current.CreateOrUpdate(obj);
```

### Deleting objects
To delete exact object use `Delete` method:

```c#
  SimpleORM.Current.Delete(obj);
```

### Conditions
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
    new DataEntityListLoadOptions(size: 1, index: 2)
  );
```

### Extras
To get SQL what has been executed (for logging etc.) you should use `AddOnRequestListener`.

Example:
```c#
  var logger = AwesomeFramework.Get<IAwesomeLoggingTool>(); //example
  SimpleORM.Current.AddOnRequestListener( (sql) => logger.Log("Executed SQL: " + sql) );
```
