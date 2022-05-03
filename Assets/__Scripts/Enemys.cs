using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemys : MonoBehaviour
{
    public Enemy[] prefabs;//used for each row
    [SerializeField] int rows = 5;//rows of enemys, typically there are 5
    [SerializeField] int columns = 11;//columns of enemys, typically there are 11
    [SerializeField] float EnemySpacing = 2;//columns of enemys, typically there are 11

    private void Awake()
    {
        for (int row = 0; row < this.rows; row++)
        {
            float totalWidth = EnemySpacing * (this.columns - 1);
            float totalHeight = EnemySpacing * (this.rows - 1);
            Vector2 center = new Vector2(-totalWidth / 2, -totalHeight / 2);
            Vector3 rowPos = new Vector3(center.x, center.y + (row * EnemySpacing), 0.0f);//need position of the row to offset the column

            for (int column = 0; column < this.columns; column++)
            {
                //Instantiate each enemy which will be which ever we are on.
                Enemy enemy = Instantiate(this.prefabs[row], this.transform);
                Vector3 position = rowPos;//set the position of invader to row
                position.x += column * EnemySpacing;//offsetting the enemys
                enemy.transform.localPosition = position;//relative to parent positioning
            }
        }
    }
}
