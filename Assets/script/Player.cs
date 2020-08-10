 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 public class Player : MonoBehaviour
 {
     #region//インスペクターで設定する
     [Header("移動速度")] public float speed;
     [Header("重力")] public float gravity;
     [Header("ジャンプ速度")] public float jumpSpeed;
     [Header("ジャンプする高さ")] public float jumpHeight;
     [Header("ジャンプする長さ")] public float jumpLimitTime;
     [Header("接地判定")] public GroundCheck ground;
     [Header("天井判定")] public GroundCheck head;
     [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;
     [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
     [Header("踏みつけ判定の高さの割合(%)")] public float stepOnRate;
     #endregion

     #region//プライベート変数
     private Animator anim = null;
     private Rigidbody2D rb = null;
     private CapsuleCollider2D capcol = null;//あはは
     private bool isGround = false;
     private bool isJump = false;
     private bool isRun = false;
     private bool isHead = false; 
       
        private bool isOtherJump = false;//あはは
     private float jumpPos = 0.0f;
       private float otherJumpHeight = 0.0f;//あはは
     private float dashTime, jumpTime;
     private float beforeKey;
    private string enemyTag = "Enemy";
     private bool isDown = false; 

     #endregion

     void Start()
     {
          //コンポーネントのインスタンスを捕まえる
          anim = GetComponent<Animator>();
          rb = GetComponent<Rigidbody2D>();
          capcol = GetComponent<CapsuleCollider2D>();
     }

     void FixedUpdate()
     {
         if(!isDown)
         {
          //接地判定を得る
          isGround = ground.IsGround();
          isHead = head.IsGround(); 

          //各種座標軸の速度を求める
          float xSpeed = GetXSpeed();
          float ySpeed = GetYSpeed();

          //アニメーションを適用
          SetAnimation();

          //移動速度を設定
          rb.velocity = new Vector2(xSpeed, ySpeed);
         }
else
    {
        rb.velocity = new Vector2(0,-gravity);
    }

     }

     /// <summary>
     /// Y成分で必要な計算をし、速度を返す。
     /// </summary>
     /// <returns>Y軸の速さ</returns>
     private float GetYSpeed()
     {
          float verticalKey = Input.GetAxis("Vertical");
          float ySpeed = -gravity;

          if (isGround)
          {
              if (verticalKey > 0 && jumpTime < jumpLimitTime)
              {
                  ySpeed = jumpSpeed;
                  jumpPos = transform.position.y; //ジャンプした位置を記録する
                  isJump = true;
                  jumpTime = 0.0f;
              }
              else
              {
                  isJump = false;
              }
          }

          //
          else if(isOtherJump)
          {
             if (jumpPos + otherJumpHeight > transform.position.y && jumpTime < jumpLimitTime && !isHead)
              {
                  ySpeed = jumpSpeed;
                  jumpTime += Time.deltaTime;
              }
              else
              {
                  isOtherJump = false;
                  jumpTime = 0.0f;
              }


          }

          //
          else if (isJump)
          {
              //上ボタンを押されている。かつ、現在の高さがジャンプした位置から自分の決めた位置より下ならジャンプを継続する
              if (verticalKey > 0 && jumpPos + jumpHeight > transform.position.y && jumpTime < jumpLimitTime && !isHead)
             {
                  ySpeed = jumpSpeed;
                  jumpTime += Time.deltaTime;
              }
              else
              {
                  isJump = false;
                  jumpTime = 0.0f;
              }
          }

          if (isJump)
          {
//              ySpeed *= jumpCurve.Evaluate(jumpTime);
          }

          return ySpeed;
     }

     /// <summary>
     /// X成分で必要な計算をし、速度を返す。
     /// </summary>
     /// <returns>X軸の速さ</returns>
     private float GetXSpeed()
     {
          float horizontalKey = Input.GetAxis("Horizontal");
          float xSpeed = 0.0f;

          if (horizontalKey > 0)
          {
              transform.localScale = new Vector3(1, 1, 1);
              isRun = true;
              dashTime += Time.deltaTime;
              xSpeed = speed;
          }
          else if (horizontalKey < 0)
          {
              transform.localScale = new Vector3(-1, 1, 1);
              isRun = true;
              dashTime += Time.deltaTime;
              xSpeed = -speed;
          }
          else
          {
              isRun = false;
              xSpeed = 0.0f;
              dashTime = 0.0f;
          }

          //前回の入力からダッシュの反転を判断して速度を変える
          if (horizontalKey > 0 && beforeKey < 0)
          {
              dashTime = 0.0f;
          }
          else if (horizontalKey < 0 && beforeKey > 0)
          {
              dashTime = 0.0f;
          }

          beforeKey = horizontalKey;
          xSpeed *= dashCurve.Evaluate(dashTime);
          beforeKey = horizontalKey;
          return xSpeed;
     }

     /// <summary>
     /// アニメーションを設定する
     /// </summary>
     private void SetAnimation()
     {
          anim.SetBool("jump", isJump);
          anim.SetBool("ground", isGround);
          anim.SetBool("run", isRun);
     }

 private void OnCollisionEnter2D(Collision2D collision)
     {
          if (collision.collider.tag == enemyTag)
        {
             //踏みつけ判定になる高さ
             float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));
             //踏みつけ判定のワールド座標
             float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

             foreach (ContactPoint2D p in collision.contacts)
             {
                 if (p.point.y < judgePos)
                 {
                     ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                     if (o != null)
                     {
                         otherJumpHeight = o.boundHeight;    //踏んづけたものから跳ねる高さを取得する
                         o.playerStepOn = true;        //踏んづけたものに対して踏んづけた事を通知する
                         jumpPos = transform.position.y; //ジャンプした位置を記録する
                         isOtherJump = true;
                         isJump = false;
                         jumpTime = 0.0f; 
                     }
                     else
                     {
                         Debug.Log("ObjectCollisionが付いてないよ!");
                     }
                 }
                 else
                 {
                     anim.Play("player_down");
                     isDown = true;
                     break;
                 }
             }
         }
     }

     


 }