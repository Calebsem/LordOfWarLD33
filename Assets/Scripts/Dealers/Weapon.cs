using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum WeaponType
{
    Handgun = 100,
    SMG = 200,
    MachineGun = 400,
    Explosives = 800
}

public enum WeaponQuality
{
    Crappy = 1,
    Factory = 2,
    CustomMade = 4
}

public class Weapon
{
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public WeaponType Type { get; set; }
    public int Price { get; set; }
    public WeaponQuality Quality { get; set; }

    public IEnumerable<Weapon> MakeStock(int amount)
    {
        var stock = new List<Weapon>();

        var weapon = new Weapon()
        {
            Guid = Guid,
            Type = Type,
            Quality = Quality,
            Price = Price,
            Name = Name
        };
        
        for(int i = 0; i < amount; i++)
        {
            stock.Add(weapon);
        }
        return stock;
    }
}

public static class WeaponProvider
{

    public static Weapon CreateWeapon(WeaponType type, WeaponQuality quality = WeaponQuality.Factory)
    {
        var weapon = new Weapon()
        {
            Guid = Guid.NewGuid(),
            Type = type,
            Quality = quality
        };

        var values = Enum.GetValues(typeof(WeaponType)).Cast<int>();
        var index = Enum.GetNames(typeof(WeaponType)).ToList().IndexOf(type.ToString()); // ugh

        var price = values.ElementAt(index);

        values = Enum.GetValues(typeof(WeaponQuality)).Cast<int>();
        index = Enum.GetNames(typeof(WeaponQuality)).ToList().IndexOf(quality.ToString()); // ugh

        price *= values.ElementAt(index);

        weapon.Price = price;
        weapon.Name = type.ToString();

        return weapon;
    }
}
