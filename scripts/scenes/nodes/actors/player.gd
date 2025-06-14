class_name Player extends CharacterActor

func _process(delta: float) -> void:
	self.direction.x = Input.get_action_strength("MOVEMENT_RIGHT") - Input.get_action_strength("MOVEMENT_LEFT")
	self.direction.y = Input.get_action_strength("MOVEMENT_DOWN") - Input.get_action_strength("MOVEMENT_UP")	
	self.move_speed_modifier = 2.0 if Input.get_action_strength("MODIFIER_SPRINT") else 0.5 if Input.get_action_strength("MODIFIER_SNEAK") else 1.0
	
	super(delta)
	
	pass
