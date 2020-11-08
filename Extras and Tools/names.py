from shutil import copyfile

color = ['Red', 'R']

for i in range(1, 14):
	name = f'{color[0]} {i}.png'
	if i < 10:
		num = f'0{i}'
	else:
		num = f'{i}'
	copyfile(name, f'{color[1]}0-{num}.png')
	copyfile(name, f'{color[1]}1-{num}.png')