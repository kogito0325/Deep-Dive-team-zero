using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hero : MonoBehaviour
{
	public GameObject spotLightPrefab;
	public GameObject bulletPrefab;
	public GameObject backGround;
	public Image blackGround;
	public GameObject blackGround2;
	public GameObject spotFlat;
	public GameObject barrier;
	public GameObject specialBulletPrefab;

	public AudioSource heroAudio;
	public AudioClip[] heroFXs;

	public Image hpBar;

	public Sprite[] status;

	public float randX_velocity;
	public float randY_velocity;

	public float bulletSpeed;

	public float maxHp;
	public float nowHp;

	private Transform target;
	private Rigidbody2D heroRigidbody;
	private Animator heroAnim;

	private bool isDead;
	public float distance;

	void Start()
	{
		heroAnim = GetComponent<Animator>();
		target = FindAnyObjectByType<PlayerController>().transform;
		heroRigidbody = GetComponent<Rigidbody2D>();
		heroAudio.volume = PlayerPrefs.GetFloat("FX", 1);
		isDead = false;

		nowHp = maxHp;

		StartCoroutine(LifeCycle());
	}

	IEnumerator LifeCycle()
	{
		while (target.GetComponent<Rigidbody2D>().linearVelocity == Vector2.zero) yield return null;
		StartCoroutine(Move());
		StartCoroutine(UpdateRandVelocity());
		StartCoroutine(SkillCycle());
	}

	IEnumerator Move()
	{
		while (!isDead)
		{
			float x_distance = Mathf.Abs(target.position.x - transform.position.x);
			float y_distance = Mathf.Abs(target.position.y - transform.position.y);

			float x_velocity = distance - x_distance;
			float y_velocity = distance - y_distance;

			x_velocity = target.position.x >= transform.position.x ? -x_velocity : x_velocity;
			y_velocity = target.position.y >= transform.position.y ? -y_velocity : y_velocity;

			x_velocity += randX_velocity;
			y_velocity += randY_velocity;

			heroAnim.SetFloat("x_velocity", x_velocity);
			heroAnim.SetFloat("y_velocity", y_velocity);

			heroRigidbody.linearVelocity = new Vector2(x_velocity, y_velocity);

			yield return null;
		}
	}
	
	public IEnumerator Hit(float damage)
	{
		heroAudio.PlayOneShot(heroFXs[1]);
		heroAnim.enabled = false;
		GetComponent<SpriteRenderer>().sprite = status[1];

		nowHp -= damage;
		hpBar.fillAmount = nowHp / maxHp;

		if(nowHp <= 0)
		{
			yield return StartCoroutine(Die());
		}

		barrier.SetActive(true);
		yield return new WaitForSeconds(1f);
		heroAnim.enabled = true;
		GetComponent<SpriteRenderer>().sprite = status[0];

		yield return new WaitForSeconds(4f);
		barrier.SetActive(false);
	}

	IEnumerator Die()
	{
		heroAudio.PlayOneShot(heroFXs[2]);
		isDead = true;
		blackGround.gameObject.SetActive(true);
		float instTime = 3f;
		float instNowTime = 0;
		while (instNowTime < instTime)
		{
			blackGround.GetComponent<Image>().color = new Color(0, 0, 0, instNowTime / instTime);
			instNowTime += Time.deltaTime;
			yield return null;
		}
		SceneManager.LoadScene(2);
	}

	IEnumerator UpdateRandVelocity()
	{
		randX_velocity = Random.Range(-distance*2, distance*2);
		randY_velocity = Random.Range(-distance*2, distance*2);
		yield return new WaitForSeconds(1f);

		StartCoroutine(UpdateRandVelocity());
	}

	IEnumerator SkillCycle()
	{
		if (!isDead)
		{
			int randInt = Random.Range(0, 9);
			switch (randInt)
			{
				case 0:
					yield return StartCoroutine(SkillSpotLight());
					break;
				case 1:
				case 2:
				case 3:
					yield return StartCoroutine(SkillShotBullet());
					break;
				case 4:
				case 5:
				case 6:
					yield return StartCoroutine(SkillSpreadBullet());
					break;
				case 7:
				case 8:
					yield return StartCoroutine(SkillShotSpecialBullet());
					break;
			}
		}

		yield return new WaitForSeconds(0.95f);
		yield return StartCoroutine(SkillCycle());
	}
	
	IEnumerator SkillSpotLight()
	{
		backGround.gameObject.SetActive(true);
		GameObject[] lights = new GameObject[3];
		barrier.SetActive(true);

		//안전장판 예고
		for(int i = 0; i < 3; i++)
		{
			float randX = Random.Range(-5f, 5f);
			float randY = Random.Range(-3f, 0.6f);

			lights[i] = Instantiate(spotLightPrefab, new Vector2(randX, randY), Quaternion.identity, backGround.transform);
		}
		
		//위험장판 페이드인
		for (float fadeTime = 0f; fadeTime < 0.2f; fadeTime += Time.deltaTime)
		{
			Color c = backGround.GetComponent<SpriteRenderer>().color;
			c.a = fadeTime * 4;
			backGround.GetComponent<SpriteRenderer>().color = c;
			yield return true;
		}
		yield return new WaitForSeconds(0.5f);

		//위험장판 페이드아웃
		for (float fadeTime = 0.2f; fadeTime > 0f; fadeTime -= Time.deltaTime)
		{
			Color c = backGround.GetComponent<SpriteRenderer>().color;
			c.a = fadeTime * 4;
			backGround.GetComponent<SpriteRenderer>().color = c;
			yield return true;
		}

		//암전
		for (float fadeTime = 0f; fadeTime < 0.2f; fadeTime += Time.deltaTime)
		{
			Color c = blackGround.color;
			c.a = fadeTime * 4;
			blackGround.color = c;
			yield return true;
		}
		yield return new WaitForSeconds(0.5f);

		//스포트라이트 발사
		heroAudio.PlayOneShot(heroFXs[0]);
		heroAnim.SetBool("spotLight", true);

		isDead = true;
		blackGround2.GetComponent<SpriteRenderer>().color = blackGround.color;
		blackGround.color = new Color(0, 0, 0, 0);
		blackGround2.SetActive(true);
		spotFlat.SetActive(true);
		spotFlat.GetComponent<SpriteRenderer>().color = Color.white;
		for (int i = 0; i < 3; i++)
		{
			lights[i].transform.GetChild(0).gameObject.SetActive(true);
		}
		if(!target.GetComponent<PlayerController>().isInSpot)
			StartCoroutine(target.GetComponent<PlayerController>().Hit());

		yield return new WaitForSeconds(1f);

		heroAnim.SetBool("spotLight", false);

		//패턴 페이드아웃
		for (float fadeTime = 1f; fadeTime > 0f; fadeTime -= Time.deltaTime)
		{
			Color cb = blackGround2.GetComponent<SpriteRenderer>().color;
			cb.a = fadeTime;
			blackGround2.GetComponent<SpriteRenderer>().color = cb;

			Color sf = spotFlat.GetComponent<SpriteRenderer>().color;
			sf.a = fadeTime;
			spotFlat.GetComponent<SpriteRenderer>().color = sf;
			yield return true;
		}

		//인스턴스 제거
		foreach (GameObject light in lights) Destroy(light);
		backGround.SetActive(false);
		blackGround2.SetActive(false);
		spotFlat.SetActive(false);
		isDead = false;
		StartCoroutine(Move());
		barrier.SetActive(false);
	}

	IEnumerator SkillShotBullet()
	{
		for (int i = 0; i < 4; i++)
		{
			Vector3 thisPos = transform.position;

			// 목표 방향 계산
			Vector2 direction = (target.position - thisPos).normalized;

			// 총알 생성
			GameObject bullet = Instantiate(bulletPrefab, thisPos, Quaternion.identity);

			// 총알에 Rigidbody2D 컴포넌트가 있다면 힘을 가합니다.
			Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
			if (rb != null)
			{
				rb.linearVelocity = direction * bulletSpeed;
			}

			// 총알 회전 (방향을 기준으로)
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

			heroAudio.PlayOneShot(heroFXs[0]);

			yield return new WaitForSeconds(0.3f);
		}
	}

	IEnumerator SkillShotSpecialBullet()
	{
		for (int i = 0; i < 2; i++)
		{
			Vector3 thisPos = transform.position;

			// 목표 방향 계산
			Vector2 direction = (target.position - thisPos).normalized;

			// 총알 생성
			GameObject bullet = Instantiate(specialBulletPrefab, thisPos, Quaternion.identity);

			// 총알에 Rigidbody2D 컴포넌트가 있다면 힘을 가합니다.
			Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
			if (rb != null)
			{
				rb.linearVelocity = direction * bulletSpeed;
			}

			// 총알 회전 (방향을 기준으로)
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

			heroAudio.PlayOneShot(heroFXs[0]);

			yield return new WaitForSeconds(0.5f);
		}
	}

	IEnumerator SkillSpreadBullet()
	{
		int oneShoting = 8;
		float angle = 360 / oneShoting;
		for(int i = 0; i < 3; i++)
		{
			for(int j =0; j < oneShoting; j++)
			{
				GameObject bullet;
				bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
				bullet.transform.Rotate(new Vector3(0f, 0f, angle * j));
				bullet.GetComponent<Rigidbody2D>().linearVelocity = bullet.transform.right * bulletSpeed;
			}
			heroAudio.PlayOneShot(heroFXs[0]);
			yield return new WaitForSeconds(1f);
		}
	}

	
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player" && barrier.activeSelf)
		{
			StartCoroutine(target.GetComponent<PlayerController>().Hit());
		}
	}
	
}
