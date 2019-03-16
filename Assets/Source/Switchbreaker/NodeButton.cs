using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// TODO: Comment script for what functions do - Michel
public class NodeButton : MonoBehaviour {
    //Types:
    private enum NodeStatus {disabled, candidate, agent}// dusabled - cannot be interacted with     candidate - can be interacted with     agent - currently engaged

    //Tweakables:
    [Header("Tweakables")]
    [SerializeField] private float _selectionRadius   = 1f;
    [SerializeField] private float _deselectionRadius = 1.2f;
    [SerializeField] private float _maxGravityForce   = 100f;
    [SerializeField] private AudioClip _activationSound;

    //Scene References:
    [Header("Scene References")]
    [SerializeField] private Transform   _selectionOrigin; //The point from which we compare which node is selected
    [SerializeField] private Rigidbody2D _puckBody; //The springy puck body
    [SerializeField] private Image       _image;
    [SerializeField] private Sprite      _agentGraphic;
    [SerializeField] private Sprite      _candidateGraphic;
    [SerializeField] private Sprite      _disabledGraphic;
    [SerializeField] private RippleParticle _ripple;

    public GameObject storage;
    public bool favorActive = true;
    public bool activatedBeforeEndOfLine = false;

    private Action _onSelected;
    private Action _onDeselected;

    //State:
    private NodeStatus _status = NodeStatus.disabled;
    public bool _selected = false;
    public Guppy identifier;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, this._selectionRadius * this.transform.lossyScale.x);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this._deselectionRadius * this.transform.lossyScale.x);
    }


    private void setStatus(NodeStatus status) {
        switch (status) {
            case NodeStatus.disabled:  this._image.sprite = this._disabledGraphic; break;
            case NodeStatus.candidate: this._image.sprite = this._candidateGraphic; break;
            case NodeStatus.agent:  this._image.sprite = this._agentGraphic;  break;
        }
        this._status = status;
    }

    public void MakeDisabled()
    {
        this.setStatus(NodeStatus.disabled);
        this._onSelected = null;
        this._onDeselected = null;
        this._selected = false;
    }

    public void MakeCandidate(Action becomeAgentAction){
        if (this._status != NodeStatus.agent)
        {
            this.setStatus(NodeStatus.candidate);
        }
        this._onSelected = becomeAgentAction;
    }
    
    public void MakeAgent(Action deselectAction)
    {
        this.setStatus(NodeStatus.agent);
        this._onDeselected = deselectAction;
    }

    public void OverrideOnDeselected(Action deselectAction)
    {
        this._onDeselected = deselectAction;
    }

    private void FixedUpdate()
    {
        //We're in a canvas scaler so we need to transform the radii:
        var scaledSelectRad   = this._selectionRadius   * this.transform.lossyScale.x;
        var scaledDeselectRad = this._deselectionRadius * this.transform.lossyScale.x;

        var distance = Vector2.Distance(this.transform.position, this._selectionOrigin.position);

        if (this._selected == false && distance <= scaledSelectRad) {
            this._selected = true;
            if (this._onSelected != null)
                this._onSelected();
        }
        else if (this._selected == true && distance > scaledDeselectRad) {
            this._selected = false;
            if (this._onDeselected != null)
                this._onDeselected();
        }
        

        //Some really Nealson-ass attraction code here:
        if(this._status != NodeStatus.disabled && distance < scaledDeselectRad) {
            var t = Mathf.InverseLerp(scaledDeselectRad, scaledSelectRad, distance);
            var forceScrub = t * 0.6f + 0.4f;
            if (t >= 0.99) forceScrub *= 1.25f;

            var forceMag = forceScrub * this._maxGravityForce * Time.fixedDeltaTime * this.transform.lossyScale.x;
            var dirToMe = (this.transform.position - this._puckBody.transform.position).normalized;
            this._puckBody.AddForce(dirToMe * forceMag);
        }
    }

    public void ActivateNode()
    {
        MehGameManager.instance.audioMan.PlayOneShot(_activationSound);
        _ripple.ActivateRipple();
        favorActive = true;
        activatedBeforeEndOfLine = true;
    }

    public void EndOfLineReached()
    {
        activatedBeforeEndOfLine = false;
    }
}
