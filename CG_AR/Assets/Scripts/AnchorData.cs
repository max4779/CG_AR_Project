using UnityEngine;

// AnchorData: CSV 파일의 한 행 데이터를 저장하는 구조체이다
public struct AnchorData
{
    // anchorId: 마커의 고유 식별자 이름을 저장한다
    public string anchorId;
    // objectName: 해당 마커 위에 배치할 오브젝트의 프리팹 이름을 저장한다
    public string objectName;

    // position: 오브젝트의 위치(x, y, z)를 저장한다
    public Vector3 position;
    // rotation: 오브젝트의 회전(x, y, z)을 저장한다
    public Vector3 rotation;
    // scale: 오브젝트의 크기(x, y, z)를 저장한다
    public Vector3 scale;
}