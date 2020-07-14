import json
import random
from threading import Thread

threshold = 0.5

#TODO: Reading file with MultiThreading
#TODO: Reduire la precision des float
#TODO: Stocker en binaire les valeurs

class Lecteur(Thread):

    def __init__(self, startingCharacter, endingCharacter):
        self.startingCharacter = startingCharacter
        self.endingCharacter = endingCharacter

    def run(self, filestream):
        pass

class IA:
    def getAction(self,foodX, foodY, playerX, playerY, enemyX, enemyY):


        obs = ((playerX - foodX, playerY - foodY), (playerX - enemyX, playerY - enemyY))

        action = q_table[obs].index(max(q_table[obs]))
        if random.random() > threshold:
            return str(random.randrange(-1, 1))+","+str(random.randrange(-1, 1))
        else:
            if action == 0:
                return "1, 1"
            elif action == 1:
                return "-1, -1"
            elif action == 2:
                return "-1, 1"
            elif action == 3:
                return "1, -1"


start_q_table = "QTable/test.txt"
f = open(start_q_table, "r")
q_table = json.loads(f.read())
q_table = eval(q_table)

# Test = IA()
# player = [8, 8]
# threshold = 0.5
# while player[0] != 3 and player[1] != 1:
#     if player[0] > 9:
#         player[0] = 9
#
#     if player[1] > 9:
#         player[1] = 9
#
#         ret = Test.getAction(3, 1, player[0], player[1], 6, 6)
#     player[0] += ret[0]
#     player[1] += ret[1]
#     print(ret)