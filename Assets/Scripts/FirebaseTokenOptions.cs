using System;

public class FirebaseTokenOptions
{
	public FirebaseTokenOptions(DateTime? notBefore = null, DateTime? expires = null, bool admin = false, bool debug = false)
	{
		this.notBefore = notBefore;
		this.expires = expires;
		this.admin = admin;
		this.debug = debug;
	}
    
	public DateTime? expires { get; private set; }
    
	public DateTime? notBefore { get; private set; }
    
	public bool admin { get; private set; }
    
	public bool debug { get; private set; }
}
