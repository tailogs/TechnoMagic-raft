using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // Префаб кубика
    public int width = 50; // Ширина области
    public int height = 50; // Высота области
    public float scale = 0.1f; // Масштаб для шума Перлина
    public float waterLevel = 0.2f; // Уровень для определения ям
    public float blockSize = 1.0f; // Размер блока

    private int[,] surfaceHeights; // Массив для хранения высоты поверхности
    private bool[,] occupiedPositions; // Массив для занятых позиций

    private TreeType[] treeTypes = new TreeType[]
    {
        new TreeType("Береза", new Color(0.8f, 0.7f, 0.5f), Color.green), // Береза
        new TreeType("Дуб", new Color(0.5f, 0.3f, 0.1f), new Color(1f, 0.84f, 0)), // Дуб
        new TreeType("Сосна", new Color(0.4f, 0.2f, 0.1f), new Color(0.0196078431f, 0.28627451f, 0.0274509804f)) // Сосна
    };

    void Start()
    {
        surfaceHeights = new int[width, height];
        occupiedPositions = new bool[width, height];
        SpawnCubes();
        AddWaterToPits();

        // Временная функция для теста
        AddTestWater();
        SpawnTrees();
    }

    void AddTestWater()
    {
        for (int x = 10; x < 15; x++)
        {
            for (int z = 10; z < 15; z++)
            {
                CreateWaterBlock(x, z, 1); // Пример воды на высоте 1
            }
        }
    }

    void SpawnCubes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float perlinValue = Mathf.PerlinNoise(x * scale, z * scale);
                int surfaceY = Mathf.FloorToInt(perlinValue * 5);
                surfaceHeights[x, z] = surfaceY;

                for (int y = surfaceY; y >= surfaceY - 2; y--)
                {
                    Vector3 position = new Vector3(x * blockSize, y * blockSize, z * blockSize);
                    GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                    cube.isStatic = true;

                    Renderer renderer = cube.GetComponent<Renderer>();
                    if (y == surfaceY)
                        renderer.material.color = new Color(0.8f, 0.5f, 0.2f);
                    else
                        renderer.material.color = Color.gray;
                }
            }
        }
    }

    void CreateWaterBlockGroup(int startX, int startZ, int surfaceY)
    {
        int waterHeight = Random.Range(1, 3); // Высота группы водных блоков
        int groupWidth = Random.Range(1, 3); // Ширина группы водных блоков

        // Добавляем больше блоков воды, если это необходимо
        for (int dx = 0; dx < groupWidth; dx++)
        {
            for (int dz = 0; dz < groupWidth; dz++)
            {
                int x = startX + dx;
                int z = startZ + dz;
                if (x >= width || z >= height) continue; // Убедимся, что не выйдем за границы

                Vector3 waterPosition = new Vector3(x * blockSize, (surfaceY - 1) * blockSize, z * blockSize); // Вода ниже поверхности
                GameObject waterCube = Instantiate(cubePrefab, waterPosition, Quaternion.identity);
                waterCube.isStatic = true;

                // Устанавливаем тег Water
                waterCube.tag = "Water";

                Renderer renderer = waterCube.GetComponent<Renderer>();
                renderer.material.color = Color.blue; // Синий цвет для воды
            }
        }
    }


    void AddWaterToPits()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                int surfaceY = surfaceHeights[x, z];

                // Добавляем воду с увеличенной вероятностью на участках с низким уровнем
                if (Random.value < 0.1f) // Вероятность появления воды
                {
                    CreateWaterBlock(x, z, surfaceY);
                }

                // Проверяем углубления для заполнения водой
                float perlinValue = Mathf.PerlinNoise(x * scale + 100, z * scale + 100);
                if (perlinValue < waterLevel)
                {
                    // Здесь создаем группу воды для углублений
                    CreateWaterBlockGroup(x, z, surfaceY);
                }
            }
        }
    }

    void CreateWaterBlock(int x, int z, int surfaceY)
    {
        Vector3 waterPosition = new Vector3(x * blockSize, (surfaceY - 1) * blockSize, z * blockSize); // Вода ниже поверхности
        GameObject waterCube = Instantiate(cubePrefab, waterPosition, Quaternion.identity);
        waterCube.isStatic = true;

        // Добавляем коллайдер для воды
        BoxCollider waterCollider = waterCube.AddComponent<BoxCollider>();
        waterCollider.size = new Vector3(blockSize, blockSize, blockSize); // Устанавливаем размер коллайдера

        // Устанавливаем тег Water
        waterCube.tag = "Water";

        Renderer renderer = waterCube.GetComponent<Renderer>();
        renderer.material.color = Color.blue; // Устанавливаем синий цвет для воды
    }

    void SpawnTrees()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                int surfaceY = surfaceHeights[x, z];
                if (Random.value < 0.03f && surfaceY > 1 && !IsAreaOccupied(x, z, 3))
                {
                    GenerateTree(x, z, surfaceY);
                }
            }
        }
    }

    void GenerateTrunk(int x, int z, int trunkHeight, int baseHeight, TreeType treeType)
    {
        for (int y = baseHeight + 1; y <= baseHeight + trunkHeight; y++) 
        {
            Vector3 trunkPosition = new Vector3(x * blockSize, y * blockSize, z * blockSize);

            // Генерация блока коры
            GameObject trunkBlock = Instantiate(cubePrefab, trunkPosition, Quaternion.identity);
            trunkBlock.isStatic = true;

            Renderer trunkRenderer = trunkBlock.GetComponent<Renderer>();
            if (trunkRenderer != null)
            {
                trunkRenderer.material.color = treeType.BarkColor; // Цвет коры
            }
        }
    }


    void GenerateTree(int x, int z, int baseHeight)
    {
        // Выбор случайного типа дерева
        TreeType treeType = treeTypes[Random.Range(0, treeTypes.Length)];

        // Генерация ствола
        int trunkHeight = Random.Range(3, 6); // Высота ствола
        GenerateTrunk(x, z, trunkHeight, baseHeight, treeType);

        // Генерация листвы
        GenerateLeaves(x, z, trunkHeight, baseHeight, treeType);
    }


    void GenerateLeaves(int x, int z, int trunkHeight, int baseHeight, TreeType treeType)
    {
        int leafRadius = Random.Range(2, 4); // Радиус кроны
        for (int dx = -leafRadius; dx <= leafRadius; dx++)
        {
            for (int dz = -leafRadius; dz <= leafRadius; dz++)
            {
                for (int dy = -1; dy <= 2; dy++) // Высота кроны над стволом
                {
                    // Условие: проверяем только расстояние по всем осям
                    float distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
                    if (distance > leafRadius || Random.value < 0.2f) continue; // Добавляем случайность для естественности

                    int leafX = x + dx;
                    int leafZ = z + dz;
                    int leafY = baseHeight + trunkHeight + dy;

                    if (leafX < 0 || leafX >= width || leafZ < 0 || leafZ >= height) continue;

                    Vector3 leafPosition = new Vector3(leafX * blockSize, leafY * blockSize, leafZ * blockSize);
                    
                    // Убираем проверку на занятость позиции
                    GameObject leafBlock = Instantiate(cubePrefab, leafPosition, Quaternion.identity);
                    leafBlock.isStatic = true;

                    Renderer renderer = leafBlock.GetComponent<Renderer>();
                    renderer.material.color = treeType.LeafColor;
                }
            }
        }
    }

    bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, Vector3.one * blockSize * 0.5f); // Размер блока
        return colliders.Length > 0;
    }

    bool IsAreaOccupied(int centerX, int centerZ, int radius)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dz = -radius; dz <= radius; dz++)
            {
                int checkX = centerX + dx;
                int checkZ = centerZ + dz;
                if (checkX < 0 || checkX >= width || checkZ < 0 || checkZ >= height) continue;

                if (occupiedPositions[checkX, checkZ]) return true;
            }
        }
        return false;
    }

    void MarkPositionOccupied(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            occupiedPositions[x, z] = true;
        }
    }

    private class TreeType
    {
        public string Name { get; private set; }
        public Color BarkColor { get; private set; }
        public Color LeafColor { get; private set; }

        public TreeType(string name, Color barkColor, Color leafColor)
        {
            Name = name;
            BarkColor = barkColor;
            LeafColor = leafColor;
        }
    }
}
