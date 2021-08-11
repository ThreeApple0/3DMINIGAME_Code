using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;

// Firebase 불러오기

public class Firebasetest : MonoBehaviour
{
    class Rank
    {
        // 순위 정보를 담는 Rank 클래스
        // Firebase와 동일하게 name, score, timestamp를 가지게 해야함
        public string name;
        
        public float goaltime;
        // JSON 형태로 바꿀 때, 프로퍼티는 지원이 안됨. 프로퍼티로 X

        public Rank(string name,  float goaltime)
        {
            // 초기화하기 쉽게 생성자 사용
            this.name = name;
            
            this.goaltime = goaltime;
        }
    }

    public DatabaseReference reference { get; set; }
    // 라이브러리를 통해 불러온 FirebaseDatabase 관련객체를 선언해서 사용

    void Start()
    {

        
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        // 데이터베이스 경로를 설정해 인스턴스를 초기화
        // Database의 특정지점을 가리킬 수 있는데, 그 중 RootReference를 가리킴

        Rank rank = new Rank("PLAYERNAME", 123.456f);
        string json = JsonUtility.ToJson(rank);
        // 데이터를 json형태로 반환

        string key = reference.Child("rank").Child("map2").Push().Key;
        
        // root의 자식 rank에 key 값을 추가해주는 것임

        reference.Child("rank").Child("map2").Child(key).SetRawJsonValueAsync(json);
        // 생성된 키의 자식으로 json데이터를 삽입
    }
}