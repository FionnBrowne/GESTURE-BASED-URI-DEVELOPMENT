using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemys : MonoBehaviour
{
    public Enemy[] prefabs;//used for each row
    [SerializeField] int rows = 5;//rows of enemys, typically there are 5
    [SerializeField] int columns = 11;//columns of enemys, typically there are 11
    [SerializeField] float EnemySpacing = 2;//columns of enemys, typically there are 11
    [SerializeField] float EnemySpeed = 3.0f;//columns of enemys, typically there are 11
    private Vector3 direction = Vector2.right;

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
    private void Update()//every frame
    {
        this.transform.position += this.EnemySpeed * Time.deltaTime * direction;//consistent movement regardless of fps

        Vector3 LEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);//translate view port cordinates to world cordinates
        Vector3 REdge = Camera.main.ViewportToWorldPoint(Vector3.right);//translate view port cordinates to world cordinates

        foreach (Transform enemy in this.transform)//loops through all child objects attached to this
        {
            if (!enemy.gameObject.activeInHierarchy)//if enemy is active or disabled
            {
                continue;
            }
            //checks if enemys have hit the edge then changes direction and decend a row
            if (direction == Vector3.right && enemy.position.x >= REdge.x - 1.0f)//-1 for padding so they dont exceed screen
            {
                DecendRow();
            }
            //checks if enemys have hit the edge then changes direction and decend a row
            else if (direction == Vector3.left && enemy.position.x <= LEdge.x + 1.0f)//1 for padding so they dont exceed screen
            {
                DecendRow();
            }
        }
    }
    private void DecendRow()
    {
        direction.x *= -1.0f;// travel to opposite side of screen
        Vector3 position = this.transform.position;//get current position
        position.y -= 1.0f;//decend enemy
        this.transform.position = position;//assign back to transform
    }
}
