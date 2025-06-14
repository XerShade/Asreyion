class_name VectorUtilities

static func get_vector_as_string(vector: Vector2):
	if(vector.y > 0):
		return "down"
	elif(vector.y < 0):
		return "up"
	elif(vector.x > 0):
		return "right"
	elif(vector.x < 0):
		return "left"
	return "down"

static func get_vector_int_as_string(vector: Vector2i):
	if(vector.y > 0):
		return "down"
	elif(vector.y < 0):
		return "up"
	elif(vector.x > 0):
		return "right"
	elif(vector.x < 0):
		return "left"
	return "down"
