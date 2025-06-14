class_name KinematicActor extends CharacterBody2D

@onready var collision_shape: CollisionShape2D = %CollisionShape
@onready var sprite: Sprite2D = %Sprite

func _physics_process(delta: float) -> void:
	if(self.velocity != Vector2.ZERO):
		self.move_and_slide()
	
	pass
