using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Snake : MonoBehaviour {
    public int xSize;
    public int ySize;
    public GameObject block;

    public Material headMaterial;
    public Material tailMaterial;
    public Material foodMaterial;

    public AudioSource coinAudioSource;

    public TMP_Text scoreText;
    int score;

    GameObject head;
    List<GameObject> tail;
    GameObject food;
    Vector2 direction = Vector2.left;

    float passedTime;
    float movementSpeedTimeDelta = 0.2f;

    private void CreateGameGrid() {
        for (int x = 0; x <= xSize; x++) {
            GameObject borderBottom = Instantiate(block) as GameObject;
            var bottomVector = new Vector3(x - xSize / 2, -ySize / 2, 0);
            borderBottom.GetComponent<Transform>().position = bottomVector;

            GameObject borderTop = Instantiate(block) as GameObject;
            var topVector = new Vector3(x - xSize / 2, ySize - ySize / 2, 0);
            borderTop.GetComponent<Transform>().position = topVector;
        }

        for (int y = 0; y <= ySize; y++) {
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<Transform>().position = new Vector3(-xSize / 2, y - (ySize / 2), 0);

            GameObject borderLeft = Instantiate(block) as GameObject;
            borderLeft.GetComponent<Transform>().position = new Vector3(xSize - (xSize / 2), y - (ySize / 2), 0);
        }
    }

    private Vector2 GetRandomPos() {
        return new Vector2(UnityEngine.Random.Range((-xSize / 2) + 1, xSize / 2), UnityEngine.Random.Range((-ySize / 2) + 1, ySize / 2));
    }

    private bool IsInGameValidGameField(Vector3 position) {
        var xSizeValue = xSize / 2;
        var ySizeValue = ySize / 2;

        var isInHoriozontalBound = IsValueInRange(((int)position.x), xSizeValue, -xSizeValue);
        var isInVerticalBound = IsValueInRange(((int)position.y), ySizeValue, -ySizeValue);

        return isInHoriozontalBound && isInVerticalBound;
    }

    private bool IsValueInRange(int value, int top, int bottom) {
        return value < top && value > bottom;
    }

    private bool ContainedInSanake(Vector2 position) {
        if (IsWrongPlace(head, position)) {
            return true;
        }

        if (tail != null) {
            foreach (var item in tail) {
                if (IsWrongPlace(item, position)) {
                    return true;
                }
            }
        }
        

        return false;
    }

    private bool IsWrongPlace(GameObject item, Vector2 position) {
        if (item == null || item.transform.position == null) {
            return false;
        }

        return position.x == item.transform.position.x && position.y == item.transform.position.y;
    }

    private Vector2 GetRandomValidPosition() {
        Vector2 position = GetRandomPos();

        while (ContainedInSanake(position))
        {
            position = GetRandomPos();
        }

        return position;
    }

    private void SpawnFood() {
        Vector2 position = GetRandomValidPosition();

        food = Instantiate(block) as GameObject;
        food.transform.position = new Vector3(position.x, position.y, 0);
        food.GetComponent<MeshRenderer>().material = foodMaterial;
    }

    private void CreatePlayer() {
        var position = GetRandomValidPosition();

        head = Instantiate(block) as GameObject;
        head.GetComponent<MeshRenderer>().material = headMaterial;
        head.transform.position = new Vector3(position.x, position.y, 0);
        tail = new List<GameObject>();
    }

    private bool IsFoodPosition(Vector3 position) {
        return food.transform.position.Equals(position);
    }

    private void MoveFood() {
        Vector2 position = GetRandomValidPosition();

        food.transform.position = new Vector3(position.x, position.y, 0);
    }

    private String ScoreText() {
        return "Score\n" + score.ToString();
    }

    // Start is called before the first frame update
    void Start() {
        // Get the camera height
        float height = Screen.height;

        // Now we get the position of the camera
        float camY = Camera.main.transform.position.y;

        // Now Calculate the bounds
        float lowerBound = camY + height / 2f;
        float upperBound = camY - height / 2f;

        //scoreText.text = score.ToString();
        scoreText.GetComponent<Transform>().position = new Vector3(125, 200, 0);
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.text = ScoreText();

        CreateGameGrid();
        CreatePlayer();
        SpawnFood();

        block.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.DownArrow)) {
            direction = Vector2.down;
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            direction = Vector2.left;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            direction = Vector2.right;
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            direction = Vector2.up;
        }

        passedTime += Time.deltaTime;

        if (movementSpeedTimeDelta < passedTime) {
            passedTime = 0;

            Vector3 newPosition = head.GetComponent<Transform>().position + new Vector3(direction.x, direction.y, 0);

            if (IsInGameValidGameField(newPosition)) {
                head.transform.position = newPosition;
            }

            if (IsFoodPosition(newPosition)) {
                MoveFood();

                coinAudioSource.Play();
                score += 1;
                scoreText.text = ScoreText();
            }
        }
    }
}
