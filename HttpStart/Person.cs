namespace HttpStart
{
    public class Person
    {
        private string _navn;
        private string _addresse;
        private string _mobil;

        public Person(string navn, string addresse, string mobil)
        {
            Navn = navn;
            Addresse = addresse;
            Mobil = mobil;
        }

        public string Navn { get => _navn; set => _navn = value; }
        public string Addresse { get => _addresse; set => _addresse = value; }
        public string Mobil { get => _mobil; set => _mobil = value; }

        public override string ToString()
        {
            return $"{{{nameof(Navn)}={Navn}, {nameof(Addresse)}={Addresse}, {nameof(Mobil)}={Mobil}}}";
        }
    }
}