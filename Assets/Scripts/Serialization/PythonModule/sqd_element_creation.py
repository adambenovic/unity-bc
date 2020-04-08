import json


counter_opaque_expression = 1
counter_occurrence_specification = 1
counter_interaction_constraint = 1


def create_new_lifeline(self, class_name):
    with open('JSONs/lifeline.json', 'r') as fp:
        json_lifeline = json.load(fp)
        json_lifeline["XmiId"] = class_name
        json_lifeline["coveredBy"].append({"XmiIdRef" : self.interaction["XmiId"]})
        json_lifeline["owner"] = {"XmiIdRef" : self.interaction["XmiId"]}
        json_lifeline["interaction"] = {"XmiIdRef" : self.interaction["XmiId"]}
        json_lifeline["name"] = class_name

        self.interaction["lifeline"].append({ "XmiIdRef" : class_name})
        self.interaction["covered"].append({"XmiIdRef": class_name})
        self.interaction["ownedElement"].append({"XmiIdRef": class_name})
        self.lifelines.append(class_name)
        self.output.append(json_lifeline)

        # print(json.dumps(json_lifeline))


def create_new_message(self, occurrence_specification_receive, occurrence_specification_send, method_name):
    with open('JSONs/message.json', 'r') as fp:
        json_message = json.load(fp)
        # message_xmiid = method_name + "()"
        message_xmiid = "mess_" + occurrence_specification_receive + "_" + occurrence_specification_send
        json_message["XmiId"] = message_xmiid
        json_message["receiveEvent"] = {"XmiIdRef" : occurrence_specification_receive}
        json_message["sendEvent"] = {"XmiIdRef": occurrence_specification_send}
        json_message["interaction"] = {"XmiIdRef": self.interaction["XmiId"]}
        json_message["owner"] = {"XmiIdRef": self.interaction["XmiId"]}
        json_message["name"] = method_name

        self.interaction["message"].append({"XmiIdRef": message_xmiid})
        self.interaction["ownedElement"].append({"XmiIdRef": message_xmiid})
        self.messages.append(message_xmiid)

        return json_message


def create_new_combined_fragment(self, interaction_operand_xmiid, interaction_operator):
    with open('JSONs/combined_fragment.json', 'r') as fp:
        json_combined_fragment = json.load(fp)
        combined_fragment_xmiid = "cf_" + interaction_operand_xmiid
        json_combined_fragment["XmiId"] = combined_fragment_xmiid
        json_combined_fragment["interactionOperator"] = interaction_operator
        json_combined_fragment["operand"].append({"XmiIdRef": interaction_operand_xmiid})
        json_combined_fragment["enclosingInteraction"] = {"XmiIdRef": self.interaction["XmiId"]}
        json_combined_fragment["owner"] = {"XmiIdRef": self.interaction["XmiId"]}
        json_combined_fragment["ownedElement"].append({"XmiIdRef": interaction_operand_xmiid})

        self.interaction["fragment"].append({"XmiIdRef": combined_fragment_xmiid})
        self.interaction["ownedElement"].append({"XmiIdRef": combined_fragment_xmiid})
        return json_combined_fragment


def create_new_interaction_operand(self, interaction_constraint_xmiid, fragments):
    with open('JSONs/interaction_operand.json', 'r') as fp:
        json_interaction_operand = json.load(fp)
        interaction_operand_xmiid = "io_" + interaction_constraint_xmiid
        json_interaction_operand["XmiId"] = interaction_operand_xmiid
        json_interaction_operand["guard"] = {"XmiIdRef" : interaction_constraint_xmiid}
        json_interaction_operand["enclosingInteraction"] = {"XmiIdRef": self.interaction["XmiId"]}

        if fragments is not None:
            for fragment in fragments:
                json_interaction_operand["ownedElement"].append({"XmiIdRef": fragment})
                json_interaction_operand["fragment"].append({"XmiIdRef": fragment})

        return json_interaction_operand


def create_new_occurrence_specification(self, class_name, method_name):
    with open('JSONs/occurrence_specification.json', 'r') as fp:
        global counter_occurrence_specification

        json_occurrence_specification = json.load(fp)
        occurrence_specification_xmiid = "oc_" + str(counter_occurrence_specification)
        json_occurrence_specification["XmiId"] = occurrence_specification_xmiid
        json_occurrence_specification["covered"].append({"XmiIdRef": class_name})
        json_occurrence_specification["enclosingInteraction"] = {"XmiIdRef": self.interaction["XmiId"]}
        json_occurrence_specification["owner"] = {"XmiIdRef": self.interaction["XmiId"]}

        self.interaction["fragment"].append({"XmiIdRef": occurrence_specification_xmiid})
        self.interaction["ownedElement"].append({"XmiIdRef": occurrence_specification_xmiid})

        counter_occurrence_specification = counter_occurrence_specification + 1
        return json_occurrence_specification


def create_new_opaque_expression(body):
    with open('JSONs/opaque_expresion.json', 'r') as fp:
        global counter_opaque_expression

        json_opaque_expression = json.load(fp)
        opaque_expression_xmiid = "oe_" + str(counter_opaque_expression)
        json_opaque_expression["XmiId"] = opaque_expression_xmiid
        json_opaque_expression["body"] = body

        counter_opaque_expression = counter_opaque_expression + 1
        return json_opaque_expression


def create_new_interaction_constraint(opaque_xmiid):
    with open('JSONs/interaction_constraint.json', 'r') as fp:
        global counter_interaction_constraint

        json_interaction_constraint = json.load(fp)
        interaction_constraint_xmiid = "ic_" + opaque_xmiid
        json_interaction_constraint["XmiId"] = interaction_constraint_xmiid
        json_interaction_constraint["specification"] = {"XmiIdRef": opaque_xmiid}

        counter_interaction_constraint = counter_interaction_constraint + 1
        return json_interaction_constraint


def create_new_interaction(self):
    with open('JSONs/interaction.json', 'r') as fp:
        json_interaction = json.load(fp)
        json_interaction["XmiId"] = self.inputFile
        self.interaction = json_interaction

        self.output.append(json_interaction)

