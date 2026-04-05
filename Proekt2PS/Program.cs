using System;
using System.Diagnostics;
using System.Threading;

public enum Rarity { Common = 1, Rare, Epic, Legendary }

public struct ItemStruct
{
    public string Name;
    public string Type;
    public int Value;
    public float Weight;
    public Rarity Rarity;
    public bool Equipped;

    public int Power() => Value * (int)Rarity;
}

public class ItemClass
{
    public string Name;
    public string Type;
    public int Value;
    public float Weight;
    public Rarity Rarity;
    public bool Equipped;

    public int Power() => Value * (int)Rarity;
}

static class RPGInventory  // Made static since all members are static
{
    static readonly string[] itemNames =  // Added readonly
    {
        "Iron Sword", "Dragon Slayer", "Shadow Dagger", "Wizard Staff",
        "Golden Armor", "Healing Potion", "Ring of Power", "Boots of Speed"
    };

    static readonly string[] itemTypes =  // Added readonly
    {
        "Weapon", "Armor", "Potion", "Accessory"
    };

    static readonly ThreadLocal<Random> rand = new ThreadLocal<Random>(() => new Random());  // Thread-safe random

    public static ItemStruct[] structInventory;
    public static ItemClass[] classInventory;

    public static void GenerateStructInventory(int count)
    {
        // Input validation
        if (count <= 0)
        {
            Console.WriteLine($"\n⚠️ Invalid count: {count}. Must be positive.");
            return;
        }

        try
        {
            structInventory = new ItemStruct[count];

            for (int i = 0; i < count; i++)
            {
                ref ItemStruct item = ref structInventory[i];  // Cache reference to avoid repeated indexing
                item.Name = itemNames[rand.Value!.Next(itemNames.Length)];
                item.Type = itemTypes[rand.Value.Next(itemTypes.Length)];
                item.Value = rand.Value.Next(20, 1000);
                item.Weight = (float)rand.Value.NextDouble() * 15f;  // Use float literal
                item.Rarity = (Rarity)rand.Value.Next(1, 5);
                // Equipped defaults to false - no need to set explicitly
            }

            Console.WriteLine($"\n✓ Struct Inventory generated with {count:N0} items.");  // Formatted number
        }
        catch (OutOfMemoryException)
        {
            Console.WriteLine($"\n⚠️ Out of memory trying to allocate {count:N0} struct items!");
            structInventory = null;
        }
    }

    public static void GenerateClassInventory(int count)
    {
        if (count <= 0)
        {
            Console.WriteLine($"\n⚠️ Invalid count: {count}. Must be positive.");
            return;
        }

        try
        {
            classInventory = new ItemClass[count];

            for (int i = 0; i < count; i++)
            {
                classInventory[i] = new ItemClass();
                var item = classInventory[i];  // Cache reference for readability
                item.Name = itemNames[rand.Value!.Next(itemNames.Length)];
                item.Type = itemTypes[rand.Value.Next(itemTypes.Length)];
                item.Value = rand.Value.Next(20, 1000);
                item.Weight = (float)rand.Value.NextDouble() * 15f;
                item.Rarity = (Rarity)rand.Value.Next(1, 5);
                // Equipped defaults to false
            }

            Console.WriteLine($"\n✓ Class Inventory generated with {count:N0} items.");
        }
        catch (OutOfMemoryException)
        {
            Console.WriteLine($"\n⚠️ Out of memory trying to allocate {count:N0} class items!");
            classInventory = null;
        }
    }

    public static void ShowStructInventory(int maxItems = 10)
    {
        if (structInventory == null || structInventory.Length == 0)
        {
            Console.WriteLine("\n⚠️ Struct Inventory is empty! Please generate inventory first (Option 1).");
            return;
        }

        // Validate maxItems
        if (maxItems <= 0)
        {
            Console.WriteLine("\n⚠️ Invalid maxItems value. Showing first 10 items instead.");
            maxItems = 10;
        }

        int itemsToShow = Math.Min(maxItems, structInventory.Length);
        if (itemsToShow == 0) return;

        Console.WriteLine($"\n-- Struct Inventory Sample (showing {itemsToShow} of {structInventory.Length}) --");

        for (int i = 0; i < itemsToShow; i++)
        {
            ref ItemStruct item = ref structInventory[i];  // Cache reference
            string symbol = GetRaritySymbol(item.Rarity, item.Equipped);
            ConsoleColor color = GetRarityColor(item.Rarity);

            Console.ForegroundColor = color;
            Console.WriteLine($"{i + 1}. {symbol} {item.Name} | {item.Type} | Power:{item.Power()} | Equipped:{item.Equipped}");
            Console.ResetColor();
        }
    }

    public static void ShowClassInventory(int maxItems = 10)
    {
        if (classInventory == null || classInventory.Length == 0)
        {
            Console.WriteLine("\n⚠️ Class Inventory is empty! Please generate inventory first (Option 2).");
            return;
        }

        if (maxItems <= 0)
        {
            Console.WriteLine("\n⚠️ Invalid maxItems value. Showing first 10 items instead.");
            maxItems = 10;
        }

        int itemsToShow = Math.Min(maxItems, classInventory.Length);
        if (itemsToShow == 0) return;

        Console.WriteLine($"\n-- Class Inventory Sample (showing {itemsToShow} of {classInventory.Length}) --");

        for (int i = 0; i < itemsToShow; i++)
        {
            var item = classInventory[i];
            if (item == null)  // Defensive null check
            {
                Console.WriteLine($"{i + 1}. [null item]");
                continue;
            }

            string symbol = GetRaritySymbol(item.Rarity, item.Equipped);
            ConsoleColor color = GetRarityColor(item.Rarity);

            Console.ForegroundColor = color;
            Console.WriteLine($"{i + 1}. {symbol} {item.Name} | {item.Type} | Power:{item.Power()} | Equipped:{item.Equipped}");
            Console.ResetColor();
        }
    }

    static ConsoleColor GetRarityColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => ConsoleColor.Gray,
            Rarity.Rare => ConsoleColor.Cyan,  // Changed from Blue for better readability
            Rarity.Epic => ConsoleColor.Magenta,
            Rarity.Legendary => ConsoleColor.Yellow,
            _ => ConsoleColor.Gray  // Default case for invalid rarities
        };
    }

    static string GetRaritySymbol(Rarity rarity, bool equipped)
    {
        // Fixed symbol logic - consistent and predictable
        string symbol = "";

        if (equipped)
            symbol = "●";

        if (rarity == Rarity.Legendary)
            symbol += equipped ? " ★" : "★";

        return symbol;
    }

    public static void EquipStructItem(int index)
    {
        if (structInventory == null)
        {
            Console.WriteLine("\n⚠️ No struct inventory generated yet!");
            return;
        }

        if (index >= 0 && index < structInventory.Length)
        {
            structInventory[index].Equipped = !structInventory[index].Equipped;
            string action = structInventory[index].Equipped ? "equipped" : "unequipped";
            Console.WriteLine($"\n✓ Item {index + 1} {action} successfully!");
        }
        else
        {
            Console.WriteLine($"\n⚠️ Invalid index! Please enter a number between 1 and {structInventory.Length}.");
        }
    }

    public static void EquipClassItem(int index)
    {
        if (classInventory == null)
        {
            Console.WriteLine("\n⚠️ No class inventory generated yet!");
            return;
        }

        if (index >= 0 && index < classInventory.Length)
        {
            if (classInventory[index] == null)
            {
                Console.WriteLine($"\n⚠️ Item at index {index + 1} is null!");
                return;
            }

            classInventory[index].Equipped = !classInventory[index].Equipped;
            string action = classInventory[index].Equipped ? "equipped" : "unequipped";
            Console.WriteLine($"\n✓ Item {index + 1} {action} successfully!");
        }
        else
        {
            Console.WriteLine($"\n⚠️ Invalid index! Please enter a number between 1 and {classInventory.Length}.");
        }
    }

    public static void DropStructItem(int index)
    {
        if (structInventory == null)
        {
            Console.WriteLine("\n⚠️ No struct inventory generated yet!");
            return;
        }

        if (index >= 0 && index < structInventory.Length)
        {
            string droppedItemName = structInventory[index].Name;

            // Shift elements left (more efficient with Span<T> in modern .NET)
            for (int i = index; i < structInventory.Length - 1; i++)
            {
                structInventory[i] = structInventory[i + 1];
            }

            Array.Resize(ref structInventory, structInventory.Length - 1);
            Console.WriteLine($"\n✅ Item '{droppedItemName}' dropped successfully! {structInventory.Length} items remaining.");
        }
        else
        {
            Console.WriteLine($"\n⚠️ Invalid index! Please enter a number between 1 and {structInventory.Length}.");
        }
    }

    public static void DropClassItem(int index)
    {
        if (classInventory == null)
        {
            Console.WriteLine("\n⚠️ No class inventory generated yet!");
            return;
        }

        if (index >= 0 && index < classInventory.Length)
        {
            if (classInventory[index] == null)
            {
                Console.WriteLine($"\n⚠️ Item at index {index + 1} is already null!");
                return;
            }

            string droppedItemName = classInventory[index].Name;

            // Shift references left
            for (int i = index; i < classInventory.Length - 1; i++)
            {
                classInventory[i] = classInventory[i + 1];
            }

            Array.Resize(ref classInventory, classInventory.Length - 1);
            Console.WriteLine($"\n✅ Item '{droppedItemName}' dropped successfully! {classInventory.Length} items remaining.");
        }
        else
        {
            Console.WriteLine($"\n⚠️ Invalid index! Please enter a number between 1 and {classInventory.Length}.");
        }
    }

    public static void RunStructTest(int count)
    {
        if (count <= 0)
        {
            Console.WriteLine($"\n⚠️ Invalid test count: {count}");
            return;
        }

        Console.WriteLine($"\n--- Running STRUCT Performance Test with {count:N0} items ---");

        // Force garbage collection to get accurate memory measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long memoryBefore = GC.GetTotalMemory(true);
        Stopwatch sw = Stopwatch.StartNew();

        GenerateStructInventory(count);

        if (structInventory == null)
        {
            Console.WriteLine("⚠️ Failed to generate struct inventory!");
            return;
        }

        int totalPower = 0;
        int legendary = 0;
        int bestPower = 0;
        string bestItem = "";

        // Cache array length for performance
        int length = structInventory.Length;

        for (int i = 0; i < length; i++)
        {
            ref ItemStruct item = ref structInventory[i];
            int power = item.Power();  // Cache Power() result

            totalPower += power;

            if (item.Rarity == Rarity.Legendary)
                legendary++;

            if (power > bestPower)
            {
                bestPower = power;
                bestItem = item.Name;
            }
        }

        sw.Stop();
        long memoryAfter = GC.GetTotalMemory(false);
        long memoryUsed = memoryAfter - memoryBefore;

        Console.WriteLine($"\n📊 STRUCT TEST RESULTS:");
        Console.WriteLine($"   Items processed: {length:N0}");
        Console.WriteLine($"   Legendary items: {legendary:N0}");
        Console.WriteLine($"   Best item: {bestItem} (Power: {bestPower:N0})");
        Console.WriteLine($"   Total power sum: {totalPower:N0}");
        Console.WriteLine($"   Time elapsed: {sw.ElapsedMilliseconds:N0} ms ({sw.ElapsedTicks:N0} ticks)");
        Console.WriteLine($"   Memory used: {memoryUsed:N0} bytes ({memoryUsed / 1024.0:F2} KB)");
        Console.WriteLine($"   Memory per item: {(double)memoryUsed / length:F2} bytes");
    }

    public static void RunClassTest(int count)
    {
        if (count <= 0)
        {
            Console.WriteLine($"\n⚠️ Invalid test count: {count}");
            return;
        }

        Console.WriteLine($"\n--- Running CLASS Performance Test with {count:N0} items ---");

        // Force garbage collection to get accurate memory measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long memoryBefore = GC.GetTotalMemory(true);
        Stopwatch sw = Stopwatch.StartNew();

        GenerateClassInventory(count);

        if (classInventory == null)
        {
            Console.WriteLine("⚠️ Failed to generate class inventory!");
            return;
        }

        int totalPower = 0;
        int legendary = 0;
        int bestPower = 0;
        string bestItem = "";

        int length = classInventory.Length;

        for (int i = 0; i < length; i++)
        {
            var item = classInventory[i];
            if (item == null) continue;  // Defensive null check

            int power = item.Power();
            totalPower += power;

            if (item.Rarity == Rarity.Legendary)
                legendary++;

            if (power > bestPower)
            {
                bestPower = power;
                bestItem = item.Name;
            }
        }

        sw.Stop();
        long memoryAfter = GC.GetTotalMemory(false);
        long memoryUsed = memoryAfter - memoryBefore;

        Console.WriteLine($"\n📊 CLASS TEST RESULTS:");
        Console.WriteLine($"   Items processed: {length:N0}");
        Console.WriteLine($"   Legendary items: {legendary:N0}");
        Console.WriteLine($"   Best item: {bestItem} (Power: {bestPower:N0})");
        Console.WriteLine($"   Total power sum: {totalPower:N0}");
        Console.WriteLine($"   Time elapsed: {sw.ElapsedMilliseconds:N0} ms ({sw.ElapsedTicks:N0} ticks)");
        Console.WriteLine($"   Memory used: {memoryUsed:N0} bytes ({memoryUsed / 1024.0:F2} KB)");
        Console.WriteLine($"   Memory per item: {(double)memoryUsed / length:F2} bytes");

        // Hint about GC pressure
        Console.WriteLine($"   ⚠️ Note: Class version creates {length:N0} heap objects vs 1 array for structs");
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "RPG Inventory System - Struct vs Class Comparison";

        // Display info about the test
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     MINI RPG INVENTORY SYSTEM - STRUCT vs CLASS COMPARISON     ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.WriteLine("\n📚 This program demonstrates the differences between:");
        Console.WriteLine("   • Struct (value type) - stored inline in arrays");
        Console.WriteLine("   • Class (reference type) - stored as separate heap objects");
        Console.WriteLine("\n💡 Performance tests with 100,000 items will show the differences!");
        Console.WriteLine("\nPress ENTER to continue...");
        Console.ReadLine();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║       MINI RPG INVENTORY SYSTEM        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("1  Generate Struct Inventory (20 items)");
            Console.WriteLine("2  Generate Class Inventory (20 items)");
            Console.WriteLine("3  Show Struct Inventory");
            Console.WriteLine("4  Show Class Inventory");
            Console.WriteLine("5  Equip/Unequip Struct Item");
            Console.WriteLine("6  Equip/Unequip Class Item");
            Console.WriteLine("7  Drop Struct Item");
            Console.WriteLine("8  Drop Class Item");
            Console.WriteLine("9  Run Struct Performance Test (100,000)");
            Console.WriteLine("10 Run Class Performance Test (100,000)");
            Console.WriteLine("11 Generate Custom Size Inventories");
            Console.WriteLine("12 Exit");

            Console.Write("\n📌 Select option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RPGInventory.GenerateStructInventory(20);
                    break;

                case "2":
                    RPGInventory.GenerateClassInventory(20);
                    break;

                case "3":
                    RPGInventory.ShowStructInventory();
                    break;

                case "4":
                    RPGInventory.ShowClassInventory();
                    break;

                case "5":
                    Console.Write("🔧 Enter item number to equip/unequip: ");
                    if (int.TryParse(Console.ReadLine(), out int structIndex) && structIndex > 0)
                        RPGInventory.EquipStructItem(structIndex - 1);
                    else
                        Console.WriteLine("\n⚠️ Invalid input! Please enter a positive number.");
                    break;

                case "6":
                    Console.Write("🔧 Enter item number to equip/unequip: ");
                    if (int.TryParse(Console.ReadLine(), out int classIndex) && classIndex > 0)
                        RPGInventory.EquipClassItem(classIndex - 1);
                    else
                        Console.WriteLine("\n⚠️ Invalid input! Please enter a positive number.");
                    break;

                case "7":
                    Console.Write("🗑️ Enter item number to drop: ");
                    if (int.TryParse(Console.ReadLine(), out int dropStructIndex) && dropStructIndex > 0)
                        RPGInventory.DropStructItem(dropStructIndex - 1);
                    else
                        Console.WriteLine("\n⚠️ Invalid input! Please enter a positive number.");
                    break;

                case "8":
                    Console.Write("🗑️ Enter item number to drop: ");
                    if (int.TryParse(Console.ReadLine(), out int dropClassIndex) && dropClassIndex > 0)
                        RPGInventory.DropClassItem(dropClassIndex - 1);
                    else
                        Console.WriteLine("\n⚠️ Invalid input! Please enter a positive number.");
                    break;

                case "9":
                    Console.Write("⚡ Running performance test with 100,000 items...\n");
                    RPGInventory.RunStructTest(100000);
                    break;

                case "10":
                    Console.Write("⚡ Running performance test with 100,000 items...\n");
                    RPGInventory.RunClassTest(100000);
                    break;

                case "11":
                    Console.Write("📦 Enter inventory size to generate: ");
                    if (int.TryParse(Console.ReadLine(), out int customSize) && customSize > 0)
                    {
                        Console.WriteLine("\nGenerating both inventories for fair comparison...");
                        RPGInventory.GenerateStructInventory(customSize);
                        RPGInventory.GenerateClassInventory(customSize);
                        Console.WriteLine($"\n✓ Generated {customSize:N0} items in both inventories!");
                    }
                    else
                    {
                        Console.WriteLine("\n⚠️ Invalid size! Please enter a positive number.");
                    }
                    break;

                case "12":
                    Console.WriteLine("\n👋 Thanks for exploring Struct vs Class differences! Goodbye!");
                    return;

                default:
                    Console.WriteLine("\n⚠️ Invalid option! Please select 1-12.");
                    break;
            }

            Console.WriteLine("\n⏎ Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}