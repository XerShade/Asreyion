class_name Player extends CharacterBody2D

@onready var sprite_2d: Sprite2D = %Sprite2D
@onready var collision_shape_2d: CollisionShape2D = %CollisionShape2D

@export var movement_speed: float = 160

func _physics_process(_delta: float) -> void:
	self.velocity = self._get_input_velocity().normalized() * self._get_movement_speed()
	
	if self.velocity != Vector2.ZERO:
		self.move_and_slide()
	pass

func _get_input_velocity() -> Vector2:
	var calculated_velocity: Vector2 = Vector2(
		Input.get_axis("MOVEMENT_LEFT", "MOVEMENT_RIGHT"),
		Input.get_axis("MOVEMENT_UP", "MOVEMENT_DOWN")
	)
	
	return calculated_velocity

func _get_movement_speed() -> float:
	var calculated_movement_speed: float = self.movement_speed
	
	if(Input.is_action_pressed("MODIFIER_SPRINT")):
		calculated_movement_speed *= 2
	
	if(Input.is_action_pressed("MODIFIER_SNEAK")):
		calculated_movement_speed /= 2
		
	return calculated_movement_speed
