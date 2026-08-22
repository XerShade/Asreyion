---
title: "Automating Minecraft Crafting Recipes with C#"
date: 2025-05-22
categories: 
  - "coding"
tags: 
  - "generator"
  - "in"
  - "minecraft"
  - "recipe"
  - "stonecutter"
  - "stripped"
---

Hey everyone,

I wanted to share a small program I wrote in C# to generate crafting recipes for stripped logs in Minecraft, specifically for the stone cutter. The idea is pretty simple: I wanted to automate the creation of these recipes for various log types to save myself from doing it manually.

### The Problem

Manually writing out these recipes felt like a huge chore, especially when there are so many different log types to consider. I figured it’d be a good opportunity to write a quick program to automate the process.

### The Code

The program defines arrays for the various wood types and log types, and then loops over them to generate a JSON file for each combination. Here’s the C# code I came up with:

```csharp
const string OUTPUT_PATH = "PATH_TO_DATAPACK\\NAMESPACE\\recipe";
const string STRIPED_LOG_TEMPLATE = "{\r\n  \"type\": \"minecraft:stonecutting\",\r\n  \"ingredient\": [\r\n    \"minecraft:{LOG_TYPE}_{WOOD_TYPE}\"\r\n  ],\r\n  \"result\": {\r\n    \"id\": \"minecraft:stripped_{LOG_TYPE}_{WOOD_TYPE}\",\r\n    \"count\": 1\r\n  }\r\n}";

string[] WOOD_TYPES = ["wood", "log"];
string[] LOG_TYPES = ["acacia", "birch", "spruce", "oak", "dark_oak", "jungle", "cherry", "mangrove", "pale_oak"];

string[] NETHER_WOOD_TYPES = ["hyphae", "stem"];
string[] NETHER_LOG_TYPES = ["crimson", "warped"];

string[] BAMBOO_WOOD_TYPES = ["block"];
string[] BAMBOO_LOG_TYPES = ["bamboo"];

foreach (string wood_type in WOOD_TYPES)
{
    foreach (string log_type in LOG_TYPES)
    {
        Console.WriteLine($"Generating stone cutting stripping recipe for 'stripped_{log_type}_{wood_type}'.");

        string recipe = STRIPED_LOG_TEMPLATE.Replace("{LOG_TYPE}", log_type).Replace("{WOOD_TYPE}", wood_type);
        string path = Path.Combine(OUTPUT_PATH, $"stripped_{log_type}_{wood_type}.json");

        StreamWriter fs = new(path, false);
        fs.WriteLine(recipe);
        fs.Close();
    }
}

foreach (string wood_type in NETHER_WOOD_TYPES)
{
    foreach (string log_type in NETHER_LOG_TYPES)
    {
        Console.WriteLine($"Generating stone cutting stripping recipe for 'stripped_{log_type}_{wood_type}'.");

        string recipe = STRIPED_LOG_TEMPLATE.Replace("{LOG_TYPE}", log_type).Replace("{WOOD_TYPE}", wood_type);
        string path = Path.Combine(OUTPUT_PATH, $"stripped_{log_type}_{wood_type}.json");

        StreamWriter fs = new(path, false);
        fs.WriteLine(recipe);
        fs.Close();
    }
}

foreach (string wood_type in BAMBOO_WOOD_TYPES)
{
    foreach (string log_type in BAMBOO_LOG_TYPES)
    {
        Console.WriteLine($"Generating stone cutting stripping recipe for 'stripped_{log_type}_{wood_type}'.");

        string recipe = STRIPED_LOG_TEMPLATE.Replace("{LOG_TYPE}", log_type).Replace("{WOOD_TYPE}", wood_type);
        string path = Path.Combine(OUTPUT_PATH, $"stripped_{log_type}_{wood_type}.json");

        StreamWriter fs = new(path, false);
        fs.WriteLine(recipe);
        fs.Close();
    }
}
```

### How It Works

The program defines several arrays for different wood types (`WOOD_TYPES`, `NETHER_WOOD_TYPES`, `BAMBOO_WOOD_TYPES`) and their corresponding log types. It then loops through each combination, replacing the placeholders in the `STRIPED_LOG_TEMPLATE` with the log and wood types, and generates a JSON file for each one.

### How to Use

1. Replace `DATAPACK_PATH` and `NAMESPACE` in the `OUTPUT_PATH` with the correct path for your datapack.

3. Run the code in the `Program.cs` file of your project.

5. It will generate all the necessary recipe files for the stripped logs!

### Why I Did This

I could’ve spent hours manually writing these recipes out, but by writing this small program, I was able to automate the process. It probably took just as long (if not longer) to write the program itself, but it was worth it to save time in the long run. Plus, if anyone else needs to do something similar, this code could come in handy!

Feel free to use or modify this script for your own Minecraft datapacks. Just make sure to adjust the paths and namespaces to suit your project.

**GitHub GIST:**  
[Download the script here](https://gist.github.com/XerShade/5ae508b2559d6f53876679f46a7b682d)

Thanks for reading!  
\-XerShade
