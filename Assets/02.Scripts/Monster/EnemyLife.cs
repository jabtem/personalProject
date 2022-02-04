using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    int life = 100;

    Transform myTr;


    public EnemyCtrl enemy;

    public MeshRenderer lifeBar;



    // Start is called before the first frame update
    void Awake()
    {
        myTr = GetComponent<Transform>();
    }

    //데미지 함수
    void Deamage(int dam, int id)
    {
        //맞은 총알의 파워를 가져와 Enemy의 life를 감소
        life -= dam;
        //로컬적인 개념으로 머트리얼 셋팅
        lifeBar.material.SetFloat("_Progress", life / 100.0f);


        // 생명력이 바닥이면 죽이자
        if (life <= 0)
        {
            // 포톤 추가
            //자신을 파괴시킨 적 네트워크 베이스의 스코어를 증가시키는 함수를 호출함
            StartCoroutine(this.SaveKillCount(id));
            enemy.EnemyDie();
        }
    }
    void OnCollision(object[] _params)
    {
        Debug.Log(string.Format("info{0} : {1}", _params[0], _params[1]));

        CreateBlood((Vector3)_params[0]);

        life -= (int)_params[1];

        lifeBar.material.SetFloat("_Progress", life / 100.0f);

        if(life <= 0)
        {
            enemy.EnemyDie();
        }
    }

    // 드럼통 폭발 몬스터 사망 처리
    public void OnCollisionBarrel(Vector3 firePos)
    {
        //혈흔 효과 함수를 호출
        CreateBlood(firePos);

        //Enemy의 life를 0 으로
        life = 0;
        //로컬적인 개념으로 머트리얼 셋팅
        lifeBar.material.SetFloat("_Progress", life / 100.0f);

    }

    // 블러드 연출을 시작해주는 코루틴함수 호출
    void CreateBlood(Vector3 pos)
    {
        //혈흔 효과를 위한 코루틴 함수 호출
        StartCoroutine(this.CreateBloodEffects(pos));
    }

    IEnumerator CreateBloodEffects(Vector3 pos)
    {
        //blood effect 생성
        //GameObject enemyblood1 = Instantiate(enemyBloodEffect, pos, Quaternion.identity) as GameObject;
        //만약 블러드 이펙트에 오브젝트 삭제 컴포넌트가 없을시...
        //Destroy(enemyblood1, 1.5f);

        //만약 혈흔 프리팹에 차일드 오브젝트를(혈흔) up 방향으로 미리 올려놨다면...
        //혈흔데칼의 생성되는 위치는 바닥에서 조금 올린 위치로 만들어야 바닥에 묻히지 않는다
        //Vector3 decalPos = myTr.position + (Vector3.up * 0.1f);

        //혈흔데칼의 회전을 Y 축으로 랜덤으로 설정
        Quaternion decalRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
        //혈흔데칼의 크기를 랜덤으로 설정
        float scale = Random.Range(1.0f, 2.5f);

        //혈흔데칼 프리팹 생성
        //Transform enemyblood2 = Instantiate(enemyBloodDecal, decalPos, decalRot) as Transform;
        //만약 혈흔 프리팹에 차일드 오브젝트를(혈흔) up 방향으로 미리 올려놨다면...
        //Transform enemyblood2 = Instantiate(enemyBloodDecal, myTr.position, decalRot) as Transform;

        //혈흔데칼의 크기를 랜덤으로 설정
        //enemyblood2.localScale = Vector3.one * scale;

        yield return null;
    }

    // 포톤 추가
    //자신을 파괴시킨 네트워크 베이스를 검색해서 스코어를 증가시켜주는 함수
    IEnumerator SaveKillCount(int firePlayerId)
    {

        //Base 태그로 지정된 모든 네트워크 베이스를 가져와 배열에 저장
        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");

        // 전체 네트워크 베이스를 검색하여 총알의 주인을 찿아줌...
        foreach (GameObject _base in bases)
        {

            //var baseCtrl = _base.GetComponent<BaseCtrl>();

            ////네트워크베이스의 playerId가 총알의 playerId와 동일한지 판단
            //if (baseCtrl != null && baseCtrl.playerId == firePlayerId)
            //{

            //    //동일한 베이스일 경우 스코어를 증가시켜줌
            //    baseCtrl.PlusKillCount();
            //    break;

            //}

        }

        yield return null;

    }
}
