class_name Player extends CharacterBody2D

@export var MOVE_SPEED: float = 100.0

func _process(delta: float) -> void:
	var direction: Vector2 = Vector2.ZERO
	direction.x = Input.get_action_strength("MOVEMENT_RIGHT") - Input.get_action_strength("MOVEMENT_LEFT")
	direction.y = Input.get_action_strength("MOVEMENT_DOWN") - Input.get_action_strength("MOVEMENT_UP")
	
	self.velocity = direction.normalized() * self.MOVE_SPEED
	
	pass

func _physics_process(delta: float) -> void:
	self.move_and_slide()
	pass
