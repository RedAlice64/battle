using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {

	public int playerDamage;

    public int hp = 50;

    public bool inRange = false;

	private Animator animator;
	private Transform target;
	private bool skipMove;
    private float coolDown;
    private float lastAttackTime;
	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;
	
	protected override void Start () {
        coolDown = 10.0f;
        hp = 50;
        lastAttackTime = Time.time;
		GameManager.instance.AddEnemyToList (this);
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
	}

	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		if (skipMove) {
			skipMove = false;
			return;
		}

		base.AttemptMove<T> (xDir, yDir);

		skipMove = true;
	}


    public void EnemyAttacked(float y,int minusHp)
    {
        if (Mathf.Abs(transform.position.y - y) < float.Epsilon)
            hp -= minusHp;
        // TODO: Memory leak!
        lock(this)
        {
            if (hp < 0)
            {
                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
    }

	public void MoveEnemy()
	{
		int xDir = 0;
		int yDir = 0;

        //if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            //yDir = target.position.y > transform.position.y ? 1 : -1;
            xDir = -1;
        //else
            //xDir = target.position.x > transform.position.x ? 1 : -1;
            xDir = -1;

        //if (Mathf.RoundToInt(target.position.x) <2  && Mathf.RoundToInt(target.position.y) <2 )
            //hp--;
        //if(hp<=0)Destroy(gameObject);

        AttemptMove <Player> (xDir, yDir);
	}

	protected override void OnCantMove <T> (T component)
	{
		Player hitPlayer = component as Player;

        if (Time.time - lastAttackTime > coolDown)
        {
            hitPlayer.LoseFood(playerDamage);
            lastAttackTime = Time.time;
        }
        else return;

		animator.SetTrigger ("enemyAttack");

		SoundManager.instance.RandomizeSfx (enemyAttack1, enemyAttack2);
	}
}
