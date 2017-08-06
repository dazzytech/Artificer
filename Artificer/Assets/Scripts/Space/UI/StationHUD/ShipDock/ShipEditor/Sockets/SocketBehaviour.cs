using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.UI.Station.Editor.Component;

namespace Space.UI.Station.Editor.Socket
{
    public class SocketBehaviour : MonoBehaviour {

    	private SocketAttributes _attributes;
    	private SocketEventController _event;
        BaseComponent _base; 
        RawImage _image;

    	// Use this for initialization
    	public void Init (BaseComponent home) {
            _base = home;

    		_attributes = new SocketAttributes();
    		_event = new SocketEventController();

            _image = gameObject.AddComponent<RawImage>();
            _event.Init(_attributes);
            _event.OnStateChange += OnStateChange;


            _image.texture = SocketTexture.open;

            gameObject.GetComponent<RectTransform>().sizeDelta 
                = new Vector2(10f, 10f);
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    	}

    	public void Tick()
    	{
    		OnStateCycle ();

            switch (state)
            {
                case SocketAttributes.SocketState.OPEN:
                    _image.texture = (Texture)SocketTexture.open;
                    gameObject.GetComponent<RectTransform>().sizeDelta 
                        = new Vector2(10f, 10f);
                    break;
                case SocketAttributes.SocketState.CLOSED:
                    _image.texture = (Texture)SocketTexture.closed;
                    gameObject.GetComponent<RectTransform>().sizeDelta 
                        = new Vector2(10f, 10f);
                    break;
                case SocketAttributes.SocketState.PENDING:
                    _image.texture = (Texture)SocketTexture.pending;
                    gameObject.GetComponent<RectTransform>().sizeDelta 
                        = new Vector2(10f, 10f);
                    break;
            }
    	}

    	void OnStateChange(SocketAttributes.SocketState newState)
    	{
    		if (newState == state) 
    			return;

    		if (!CheckValidStatePair (newState))
    			return;

    		previous = state;
    		
    		state = newState;

    		switch (newState) {
    		case SocketAttributes.SocketState.OPEN:
    			if(previous == SocketAttributes.SocketState.CLOSED)
    			{
    				isMale = false;
    				
    				if(connectedSocket.state
    				   != SocketAttributes.SocketState.OPEN)
    				{
                        connectedSocket.Wipe();
    				}
    				
    				connectedSocket = null;
    			}
    			else if(previous == SocketAttributes.SocketState.PENDING)
    			{
    				if(connectedSocket.connectedSocket == this)
    					if(connectedSocket.state
    					   != SocketAttributes.SocketState.OPEN)
    					{
                            connectedSocket.Wipe();
    					}
    				
    				connectedSocket = null;
    			}
    			break;
    		case SocketAttributes.SocketState.CLOSED:
    			if(connectedSocket.state
    			   != SocketAttributes.SocketState.CLOSED)
    			{
                    connectedSocket.Connect();
    				isMale = true;
    			}
    			else
    				isMale = false;
    			break;

    		case SocketAttributes.SocketState.PENDING:
    			if(connectedSocket.state
    			   != SocketAttributes.SocketState.PENDING)
    			{
                    connectedSocket.CreatePending(this);
    			}
    			break;
    		}
    	}

    	bool CheckValidStatePair(SocketAttributes.SocketState newState)
    	{
    		switch (newState) {
    		case SocketAttributes.SocketState.OPEN:
    			if(state == SocketAttributes.SocketState.OPEN)
    				return false;
    			else
    				return true;
    		case SocketAttributes.SocketState.CLOSED:
    			if(state == SocketAttributes.SocketState.OPEN ||
    			   state == SocketAttributes.SocketState.CLOSED)
    				return false;
    			else
    				return true;
    		case SocketAttributes.SocketState.PENDING:
    			if(state == SocketAttributes.SocketState.PENDING ||
    			   state == SocketAttributes.SocketState.CLOSED)
    			   	return false;
    			else
    			   	return true;
    		}
    		return false;
    	}

    	void OnStateCycle()
    	{
    		switch (state) {
    		case SocketAttributes.SocketState.CLOSED:
    			if (isMale)
    				SnapToSocket (position, connectedSocket.position);
    			break;
    		case SocketAttributes.SocketState.PENDING:
    			if(connectedSocket.connectedSocket != this)
    			{
    				OnStateChange(SocketAttributes.SocketState.OPEN);
    			}
    			break;
    		}
    	}

    	/// <summary>
    	/// Snaps to socket.
    	/// Moves the parent object
    	/// the distance between the two
    	/// selected nodes
    	/// </summary>
    	/// <param name="sourceSocket">Source socket.</param>
    	/// <param name="destinationSocket">Destination socket.</param>
    	public void SnapToSocket(Vector3 sourceSocket, Vector3 destinationSocket)
    	{
    		Vector3 snapDistance = new Vector3 ();

    		snapDistance = destinationSocket - sourceSocket;

    		_base.SnapMove(snapDistance);	
    	}

    	/// <summary>
    	/// Sets male state
    	/// When parent is being dragged then 
    	/// socket becomes female and connectedSocket
    	/// becomes male. 
    	/// </summary>
    	/// <param name="isMale">If set to <c>true</c> is male.</param>
    	public void SetMale()
    	{
    		if (state == SocketAttributes.SocketState.CLOSED) 
    		{
    			isMale = true;
    			connectedSocket.isMale = false;
    		}
    	}

        public void ConnectSocketTo(SocketBehaviour other)
        {

        }

    	// GET & SET ACCESSORS
    	// READ ONLY --------------------------------------------------------------------|
    	public Vector2 position
    	{
            get 
            { 
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
    	}
    	public BaseComponent container
    	{
    		get { return _base;}
    	}

        public bool connected
        {
            get { return _attributes.state == SocketAttributes.SocketState.CLOSED; }
        }

        public bool pending
        {
            get { return _attributes.state == SocketAttributes.SocketState.PENDING; }
        }

        public float distance
        {
            get { return Vector2.Distance(position, connectedSocket.position);}
        }

    	// ------------------------------------------------------------------------------|
    	
    	// READ & WRITE -----------------------------------------------------------------|
    	public SocketBehaviour connectedSocket
    	{
    		get 
            { 
                if(_attributes == null)
                    return null;

                return _attributes.connectedSocket;
            }
    		set { _attributes.connectedSocket = value; }
    	}
    	
    	public bool isMale
    	{
    		get { return _attributes.isMale;}
    		set { _attributes.isMale = value; }
    	}

    	public SocketAttributes.SocketState state
    	{
    		get 
            { 
                if(_attributes == null)
                    return new SocketAttributes.SocketState();

                return _attributes.state;
            }
    		set { _attributes.state = value;}
    	}

    	public SocketAttributes.SocketState previous
    	{
    		get 
            { 
                if(_attributes == null)
                    return new SocketAttributes.SocketState();

                return _attributes.previous;
            }
    		set { _attributes.previous = value;}
    	}

        public string SocketID
        {
            get { return _attributes.socketID;}
            set { _attributes.socketID = value;}
        }

        public int ObjectID
        {
            get { return _base.ShipComponent.InstanceID;}
        }

        public SocketAttributes.Alignment alignment
        {
            get { return _attributes.alignment;}
            set { _attributes.alignment = value;} 
        }

    	// -------------------------------------------------------------------------------|

        public void Wipe()
        {
            if(_event != null)
                _event.Wipe();
        }

        public void Connect()
        {
            _event.Connect();
        }

        public void CreatePending(SocketBehaviour other)
        {
            _event.CreatePending(other);
        }
    }
}
