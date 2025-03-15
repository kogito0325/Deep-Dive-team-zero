using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	public float speed;  // 이동 속도
	public float unHitTime;  // 무적시간

	public bool isHit;  // 한 대 맞은 상태
	public bool isInSpot;  // 스포트라이트 안에 있는 상태

	// 노트 패턴
	public GameObject notePattern;  // 노트 패턴 UI
	
	public Image timerUI;  // 노트 패턴 시간 UI
	public Image[] notes;  // 화면에 출력되는 노트들
	public Sprite[] noteImages;  // 노트 이미지들(상, 하, 우, 좌)

	public int[] noteNumbers = new int[5];  // 다섯개의 노트를 담는 리스트

	public float noteTimer;  // 노트 패턴 시간
	public float nowNoteTimer;  // 현재 흐른 노트 패턴 시간

	// 오수 패턴
	public GameObject osuPattern;  // 오수 패턴 UI
	public GameObject[] scoreImages;  // 오수 패턴 판정 이미지 {perfact, good, bad, miss}

	public Transform osuTransform;  // 줄어드는 링

	public float countTime;  // 오수 패턴 시간
	public float minTiming;  // 최대 판정 시간
	public float maxTiming;  // 최소 판정 시간

	private int fullDamage;  // 오수 패턴으로 확보한 총 데미지

	// 기타 properties
	public GameObject hpUI;  // 용사 체력 UI
	public GameObject hurtUI;  // isHit 일 때 활성화되는 화면 가장자리 UI
	public GameObject spaceBar;  // 스페이스 바 UI
	public Sprite[] status;  // 플레이어 상태에 따른 스프라이트 {0: 평상 시, 1: 피격 시}

	public RectTransform mask;  // 페이드 인아웃 마스크

	public AudioSource playerAudioSource;  // 플레이어 오디오 스피커
	public AudioClip[] playerFxs;  // 플레이어 효과음 오디오 클립

	public float touchTime;  // 접촉 후 패턴 돌입 가능 시간

	public JoyStick joyStick;
	public int nowMobileNum;
	public GameObject ArrowsMoblie;

	Rigidbody2D playerRigidbody;  // rigidBody 컴포넌트

	private bool isProcessing;
	private int[] mobileJoystickPos = new int[2] { -850, 850 };
	private int[] mobileArrowsPos = new int[2] { 550, -550 };


	void Start()
	{
		Time.timeScale = 1;
		playerRigidbody = GetComponent<Rigidbody2D>();
		isHit = false;
		isInSpot = false;
		unHitTime = 0;
		hpUI.SetActive(true);
		playerAudioSource.volume = PlayerPrefs.GetFloat("FX", 1);
		isProcessing = false;
		nowMobileNum = 4;

		int handIdx = PlayerPrefs.GetInt("HandPos");
		joyStick.GetComponent<RectTransform>().anchoredPosition = new Vector2(mobileJoystickPos[handIdx], 300);
		ArrowsMoblie.GetComponent<RectTransform>().anchoredPosition = new Vector2(mobileArrowsPos[handIdx], -250);

		StartCoroutine(Move());  // 이동 코루틴 실행
		StartCoroutine(CircleFadeIn());  // 페이드 인
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// 영웅과 접촉 시 판정
		if (collision.gameObject.tag == "Hero")
		{
			if (!collision.gameObject.GetComponent<Hero>().barrier.activeSelf)
				StartCoroutine(Touch());
		}
	}

    IEnumerator Touch()
    {
        // 접촉 시 실행되는 코루틴
		if (!isProcessing)
		{
			isProcessing = true;
			touchTime = 0.5f;
			spaceBar.SetActive(true);
			while (touchTime > 0)
			{
				if (Input.GetKeyDown(KeyCode.Space)
					|| joyStick.gameObject.activeSelf && 
					((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
					|| Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began))
				{
					// touchTime 안에 스페이스바 눌러야 패턴 시작
					FindAnyObjectByType<Hero>().heroAudio.PlayOneShot(FindAnyObjectByType<Hero>().heroFXs[2]);
					Time.timeScale = 0;
					yield return new WaitForSecondsRealtime(0.05f);
					yield return StartCoroutine(NotePattern()); // 노트 패턴 시작 -> 성공 시 오수 패턴
					Time.timeScale = 1;
					break;
				}
				touchTime -= Time.deltaTime;
				yield return null;
			}
			spaceBar.SetActive(false);
			isProcessing = false;
		}
    }

    IEnumerator NotePattern()
	{
		fullDamage = 0;
		hpUI.SetActive(false);
		notePattern.SetActive(true);
		if (joyStick.gameObject.activeSelf) ArrowsMoblie.SetActive(true);

		for (int i = 0; i < 5; i++)
		{
			int randint = Random.Range(0, 4);
			notes[i].color = new Color(255, 255, 255, 1);
			notes[i].sprite = noteImages[randint];
			noteNumbers[i] = randint;
		}
		int noteOrder = 0;
		nowNoteTimer = noteTimer;

		if (joyStick.gameObject.activeSelf) yield return new WaitForSecondsRealtime(0.2f);

		while (nowNoteTimer > 0)
		{
			int nowKey = 4;
			if (joyStick.gameObject.activeSelf)
			{
				if (0 <= nowMobileNum && nowMobileNum <= 3)
					nowKey = nowMobileNum;
			}
			else
			{
				if (Input.GetKeyDown(KeyCode.UpArrow)) nowKey = 0;
				else if (Input.GetKeyDown(KeyCode.DownArrow)) nowKey = 1;
				else if (Input.GetKeyDown(KeyCode.RightArrow)) nowKey = 2;
				else if (Input.GetKeyDown(KeyCode.LeftArrow)) nowKey = 3;
			}

			if (0 <= nowKey && nowKey <= 3)
			{
                if (nowKey == noteNumbers[noteOrder])
                {
                    playerAudioSource.PlayOneShot(playerFxs[1]);
                    notes[noteOrder].color = new Color(255, 255, 255, 0.5f);
                    noteOrder++;
                }
				else
				{
					break;
				}
            }
			nowNoteTimer -= Time.unscaledDeltaTime;
			if (noteOrder >= 5)
			{
				playerAudioSource.PlayOneShot(playerFxs[2]);
				for (int i = 0; i < 3; i++)
				{
					playerAudioSource.PlayOneShot(playerFxs[3]);
					yield return StartCoroutine(OsuPattern());
				}
				break;
			}
			timerUI.fillAmount = nowNoteTimer / noteTimer;
			nowMobileNum = 4;
			yield return null;
		}
		nowMobileNum = 4;
		notePattern.SetActive(false);
		if (joyStick.gameObject.activeSelf) ArrowsMoblie.SetActive(false);
		hpUI.SetActive(true);
		StartCoroutine(FindAnyObjectByType<Hero>().Hit(fullDamage));
		StartCoroutine(Blink(2f));
	}

	IEnumerator OsuPattern()
	{
		float nowTime = countTime;
		osuPattern.SetActive(true);

		int scoreIdx = 4;

		foreach(GameObject img in scoreImages)
		{
			img.SetActive(false);
		}

		while (nowTime >= 0)
		{
			nowTime -= Time.unscaledDeltaTime;
			osuTransform.localScale = new Vector3(nowTime, nowTime, nowTime);
			if (Input.GetKeyDown(KeyCode.Space)
				|| joyStick.gameObject.activeSelf && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
			{
				if (Mathf.Abs(((minTiming + maxTiming) / 2) - nowTime) <= 0.05f)
				{
					// Perfact
					fullDamage += 5;
					scoreIdx = 0;
				}
				else if (Mathf.Abs(((minTiming + maxTiming) / 2) - nowTime) <= 0.1f)
				{
					// Good
					fullDamage += 3;
					scoreIdx = 1;
				}
				else if (Mathf.Abs(((minTiming + maxTiming) / 2) - nowTime) <= 0.5f)
				{
					// Bad 
					fullDamage += 1;
					scoreIdx = 2;
				}
				else
				{
					// Miss
					fullDamage += 0;
					scoreIdx = 3;
				}
				break;
			}
			yield return null;
		}
		if (nowTime < 0f) scoreIdx = 3;
		scoreImages[scoreIdx].gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(Random.Range(1, 5) * 0.2f);

		osuPattern.SetActive(false);
	}

	public IEnumerator Hit()
	{
		if (unHitTime <= 0 && isHit)
		{
			GetComponent<Animator>().enabled = false;
			GetComponent<SpriteRenderer>().sprite = status[1];
			StartCoroutine(Die());
		}
		else if (unHitTime <= 0 && !isHit)
		{
			playerAudioSource.PlayOneShot(playerFxs[0]);
			StartCoroutine(Blink());
			StartCoroutine(WaitForRecover());
			isHit = true;
		}
		yield return null;
	}

	IEnumerator Die()
	{
		playerAudioSource.PlayOneShot(playerFxs[0]);
		speed = 0;
		hurtUI.SetActive(false);
		tag = "Finish";
		yield return StartCoroutine(CircleFadeOut());
		SceneManager.LoadScene(1);
	}

	IEnumerator CircleFadeIn()
	{
		float during = 1f;
		float nowD = during;
		float size = 9000;

		//   Ŀ    ߾           Ǿ   ִٸ , anchoredPosition   (0, 0)         
		mask.anchoredPosition = Vector2.zero;

		while (nowD > 0)
		{
			nowD -= Time.deltaTime;

			//   ũ    ߾                          ġ     (Screen Space - Overlay     )
			Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
			Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
			Vector2 relativePos = playerScreenPos - screenCenter;

			//   Ŀ    ߾ӿ       Ƿ , relativePos    ״      
			mask.anchoredPosition = relativePos;
			float t = (during - nowD) / during;
			mask.sizeDelta = new Vector2(t * size, t * size);
			yield return null;
		}
	}

	IEnumerator CircleFadeOut()
	{
		float during = 1f;
		float nowD = 0f;
		float size = 9000;

		//   Ŀ    ߾           Ǿ   ִٸ , anchoredPosition   (0, 0)         
		mask.anchoredPosition = Vector2.zero;

		while (nowD < during)
		{
			nowD += Time.deltaTime;

			//   ũ    ߾                          ġ     (Screen Space - Overlay     )
			Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
			Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
			Vector2 relativePos = playerScreenPos - screenCenter;

			//   Ŀ    ߾ӿ       Ƿ , relativePos    ״      
			mask.anchoredPosition = relativePos;

			float t = during - nowD;
			mask.sizeDelta = new Vector2(t * size, t * size);

			yield return null;
		}
	}

	IEnumerator WaitForRecover()
	{
		yield return new WaitForSeconds(10f);
		if (isHit) isHit = false;
	}

	public IEnumerator Blink()
	{
		GetComponent<Animator>().enabled = false;
		GetComponent<SpriteRenderer>().sprite = status[1];
		unHitTime = 1f;
		while (unHitTime > 0)
		{
			unHitTime -= 0.05f;
			GetComponent<SpriteRenderer>().enabled = GetComponent<SpriteRenderer>().enabled ? false : true;
			yield return new WaitForSeconds(0.05f);
		}
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<SpriteRenderer>().sprite = status[0];
		GetComponent<Animator>().enabled = true;
	}

	public IEnumerator Blink(float endureTime)
	{
		GetComponent<Animator>().enabled = false;
		GetComponent<SpriteRenderer>().sprite = status[1];
		unHitTime = endureTime;
		while (unHitTime > 0)
		{
			unHitTime -= 0.05f;
			GetComponent<SpriteRenderer>().enabled = GetComponent<SpriteRenderer>().enabled ? false : true;
			yield return new WaitForSeconds(0.05f);
		}
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<SpriteRenderer>().sprite = status[0];
		GetComponent<Animator>().enabled = true;
	}

	IEnumerator Move()
	{
		while (true)
		{
			if(unHitTime <= 0.5f)
			{
				float x_speed;
				float y_speed;

				float x_input;
				float y_input;

				if (joyStick.gameObject.activeSelf)
				{
					Vector2 input = joyStick.inputVector;
					x_input = input.x;
					y_input = input.y;
				}
				else
				{
					x_input = Input.GetAxis("Horizontal");
					y_input = Input.GetAxis("Vertical");
				}
				x_speed = speed * x_input;
				y_speed = speed * y_input;

				playerRigidbody.linearVelocity = new Vector2(x_speed, y_speed);

				Animator playerAnim = GetComponent<Animator>();
				playerAnim.SetFloat("x_velocity", x_input);
				playerAnim.SetFloat("y_velocity", y_input);

				hurtUI.SetActive(isHit ? true : false);
			}
			yield return null;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.tag == "Spot")
		{
			isInSpot = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Spot")
		{
			isInSpot = false;
		}
	}
}
