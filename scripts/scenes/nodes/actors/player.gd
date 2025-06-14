class_name Player extends CharacterActor

@export var move_speed: float = 100.0
@export var cardinal_direction: Vector2 = Vector2.DOWN
@export var direction: Vector2 = Vector2.ZERO
@export var state: String = "idle"
@export var idle_animated: bool = false

func _process(delta: float) -> void:
	self.direction.x = Input.get_action_strength("MOVEMENT_RIGHT") - Input.get_action_strength("MOVEMENT_LEFT")
	self.direction.y = Input.get_action_strength("MOVEMENT_DOWN") - Input.get_action_strength("MOVEMENT_UP")
	
	self.velocity = self.direction.normalized() * self.move_speed
	
	if(self._set_state() || self._set_direction() || self._set_move_speed()):
		self._update_animation()
	
	pass

func _physics_process(delta: float) -> void:
	self.move_and_slide()
	pass

func _set_direction() -> bool:
	if(self.direction == Vector2.ZERO):
		return false
	
	var new_direction: Vector2 = self.cardinal_direction
	
	if (self.direction.y != 0):
		new_direction = Vector2.DOWN if self.direction.y > 0 else Vector2.UP
	elif (self.direction.x != 0):
		new_direction = Vector2.RIGHT if self.direction.x > 0 else Vector2.LEFT
	
	if (new_direction != self.cardinal_direction):
		self.cardinal_direction = new_direction
		return true
	
	return false

func _set_state() -> bool:
	var new_state: String = "idle" if self.direction == Vector2.ZERO else "movement"
	
	if(new_state != self.state):
		self.state = new_state
		return true
	
	return false

func _set_move_speed() -> bool:
	var new_move_speed: float = self.move_speed
	
	new_move_speed = 200 if Input.get_action_strength("MODIFIER_SPRINT") else 50 if Input.get_action_strength("MODIFIER_SNEAK") else 100
	
	if(new_move_speed != self.move_speed):
		self.move_speed = new_move_speed
		return true
	
	return false

func _update_animation() -> void:
	var animation_speed: float = self.move_speed / 25;
	var animation_name: String = self.state + "_" + VectorUtilities.get_vector_as_string(self.cardinal_direction)
	
	if(self.state == "idle" && self.idle_animated):
		animation_name += "_animated"
		animation_speed = 100
	
	self.animation_player.play(animation_name, -1, animation_speed)
	pass
