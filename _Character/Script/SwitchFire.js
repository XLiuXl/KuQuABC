

var texture : Texture [];
var Speed : int = 80;

private var FireNum : float = 1;


function Update () 
{
	FireNum += Time.deltaTime * 80;

	
	
	this.GetComponent.<Renderer>().material.SetTexture("_MainTex", texture[FireNum%20]);

}