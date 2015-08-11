
    class POC : Attribute
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        private string _otherName;
        public string OtherName
        {
            get
            {
                return _otherName;
            }

            set
            {
                _otherName = value;
            }
        }

        public POC() { }

        public POC(string _n = "", string x = "")
        {
            Name = _n;
            OtherName = x;
        }
    }

    class ModelXYZ
    {
        [POC(Name = "Nombre", OtherName = "Nome")]
        public string Name { get; set; }

        [POC(Name = "Idade", OtherName = "Idade")]
        public int Age { get; set; }
    }

    class Program
    {

        static PropertyInfo ReadAttributes(ref ModelXYZ model, string valueAttribute)
        {
            return (from p in model.GetType().GetProperties()
                     from it in p.GetCustomAttributes(typeof(POC), true)
                     where ((POC)it).Name == valueAttribute
                     select p).FirstOrDefault();
        }

        static void Main(string[] args)
        {
            ModelXYZ x = new ModelXYZ() { Name = "Lucas", Age = 21 };
            var prop = ReadAttributes(ref x, "Idade");
            var idade = prop.GetValue(x);
            Console.Write(idade);
            Console.ReadKey();
        }
    }
