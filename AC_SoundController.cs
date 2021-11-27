using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AC_Sound
{
    public string name;
    public AudioClip audioClip;

    public AC_Sound(string _name, AudioClip _audioClip)
    {
        name = _name;
        audioClip = _audioClip;
    }
}

public class AC_SoundController : MonoBehaviour
{
    #region 사운드 컨트롤러

    public static AC_SoundController instance;

    private void Awake()
    {
        instance = this;
        Initialize();
        SoundControll("BGM");
    }

    public void Initialize()
    {
        for (int i = 0; i < common_clips.Length; i++)
        {
            var bgm = act_clips.Find(x => x.name == common_clips[i].name);
            if (bgm != null) continue;

            act_clips.Add(common_clips[i]);
        }
    }

    [SerializeField] public AC_Sound[] common_clips;
    [SerializeField] public List<AC_Sound> act_clips = new List<AC_Sound>();
    public AudioSource BGM;
    public AudioSource Narration;
    [SerializeField] public List<AudioSource> Effects = new List<AudioSource>();
    public AudioSource Etc;
    public AudioSource Common;

    float prevVolume = -1f;

    public void SoundControll(string name, SoundAct soundAct = SoundAct.Play)
    {
        AudioSource target = Etc; // 임시 처리
        // 효과음인지 확인
        bool isEffect = false;
        bool isNarr = false;

        var sound = act_clips.Find(x => x.name == name);

        if (sound != null)
        {
            // BGM 실행
            if (sound.name.Contains("BGM"))
            {
                target = BGM;

                // 루프 체크
                if (!target.loop) target.loop = true; 
            }
            else if (sound.name.Contains("Narr"))
            {
                target = Narration;

                // 나레이션 정책 확인 (나레이션 재생할 때만)
                isNarr = true;
            }
            else if (sound.name.Contains("Eff"))
            {
                // 효과음인지 미리 체크
                isEffect = true;
            }
            else if (sound.name.Contains("Common"))
            {
                target = Common;
            }

            // 문제없으면 타겟 오디오소스에 클립 삽입
            target.clip = sound.audioClip;
        }
        else
        {
            // 검색되지 않았을 때 처리

            if (name.Contains("BGM"))
            {
                target = BGM;
            }
            else if (name.Contains("Narr"))
            {
                // UIController에서 공통으로 나레이션 제어할 때 사용
                target = Narration;
            }
            else if (name.Contains("Eff"))
            {
                isEffect = true;
            }
            else
            {
                // 규칙에 맞지 않는 오디오는 실행X
                return;
            }
        }

        // 나레이션일 때만
        if (isNarr)
        {
            StopAllCoroutines();
            StartCoroutine(CoNarrCallback(target.clip.length));
        }

        if (isEffect)
        {
            bool isChecked = false;

            switch (soundAct)
            {
                case SoundAct.Play:
                    for (int i = 0; i < Effects.Count; i++)
                    {
                        if (!Effects[i].isPlaying)
                        {
                            isChecked = true;

                            Effects[i].clip = target.clip;
                            target = Effects[i];
                            break;
                        }
                    }

                    if (!isChecked)
                    {
                        // Effect Audio source 추가 생성
                        var effect = Instantiate(Effects[0]);
                        effect.name = Effects.Count + "";
                        effect.transform.parent = Effects[0].transform.parent;
                        effect.clip = target.clip;
                        effect.loop = false;

                        Effects.Add(effect);
                        target = effect;
                    }

                    target.Play();
                    break;
                case SoundAct.Stop:
                    if (sound != null)
                    {
                        // 동일한 클립을 찾아서 정지
                        for (int i = 0; i < Effects.Count; i++)
                        {
                            if (Effects[i].clip != null)
                            {
                                if (Effects[i].clip.name.Equals(sound.audioClip.name) || Effects[i].clip.name.Equals(sound.name))
                                {
                                    isChecked = true;

                                    target = Effects[i];
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 동일한 클립이 없을시 효과음 전체 정지
                        for (int i = 0; i < Effects.Count; i++)
                        {
                            Effects[i].Stop();
                        }
                    }

                    if (isChecked) target.Stop();

                    break;

                case SoundAct.Pause:
                    if (sound != null)
                    {
                        // 동일한 클립을 찾아서 일시정지
                        for (int i = 0; i < Effects.Count; i++)
                        {
                            if (Effects[i].clip != null)
                            {
                                if (Effects[i].clip.name.Equals(sound.audioClip.name) || Effects[i].clip.name.Equals(sound.name))
                                {
                                    isChecked = true;

                                    target = Effects[i];
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 동일한 클립이 없을시 효과음 전체 일시정지
                        for (int i = 0; i < Effects.Count; i++)
                        {
                            Effects[i].Pause();
                        }
                    }

                    if (isChecked) target.Pause();

                    break;
                case SoundAct.UnPause:
                    if (sound != null)
                    {
                        // 동일한 클립을 찾아서 일시정지 해제
                        for (int i = 0; i < Effects.Count; i++)
                        {
                            if (Effects[i].clip != null)
                            {
                                if (Effects[i].clip.name.Equals(sound.audioClip.name) || Effects[i].clip.name.Equals(sound.name))
                                {
                                    isChecked = true;

                                    target = Effects[i];
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        isChecked = false;

                        // 동일한 클립이 없을시 효과음 전체 일시정지 해제
                        for (int i = 0; i < Effects.Count; i++)
                        {
                            Effects[i].UnPause();
                        }
                    }

                    if (isChecked) target.UnPause();

                    break;
            }
        }
        else
        {
            // 효과음이 아닐때는 타겟팅
            switch (soundAct)
            {
                case SoundAct.Play:
                    target.Play();
                    break;

                case SoundAct.Stop:
                    target.Stop();
                    break;

                case SoundAct.Pause:
                    target.Pause();
                    break;
                case SoundAct.UnPause:
                    target.UnPause();
                    break;
            }
        }
        // 재생 타입에 따라 처리
    }

    IEnumerator CoNarrCallback(float duration)
    {
        prevVolume = BGM.volume;
        BGM.DOFade(0.05f, 0.5f);
        yield return new WaitForSeconds(duration);
        BGM.DOFade(prevVolume <= 0.06f ? 1f :  prevVolume, 0.5f);
    }

    // 필요시 스크립트로 사운드 추가
    public void AddClip(string name, AudioClip clip)
    {
        var sound = FindClip(name);
        if (sound != null)
        {
            sound.audioClip = clip;
            return;
        }

        act_clips.Add(new AC_Sound(name, clip));
    }
    // 오디오 클립 길이 반환
    public float GetDuration(string name)
    {
        var sound = act_clips.Find(x => x.name == name);

        if (sound == null) return 0;
        if (sound.audioClip == null) return 0;

        return sound.audioClip.length;
    }
    public AC_Sound FindClip(string name)
    {
        var sound = act_clips.Find(x => x.name == name);
        if (sound == null) return null;

        return sound;
    }
    #endregion

    public enum SoundAct { Play, Stop, Pause, UnPause };
}